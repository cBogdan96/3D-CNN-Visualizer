import asyncio
import cProfile
import pstats
import datetime
import gc
import io
import math
import msgpack
import sys
import threading
import time

import cv2
import numpy as np
import tensorflow as tf
from numpy import expand_dims
from PIL import Image
from pycallgraph import PyCallGraph
from pycallgraph.output import GraphvizOutput
from multiprocessing import Process, Queue, Semaphore
from struct import pack
from collections import Iterable
import traceback

# # Width / height for the image to be processed.
# IMAGE_SIZE = 224
# Host used for communication.
HOST = '127.0.0.1'
# Base port for communication.
PORT = 60000
# Number of predictions to make.
PREDICTIONS_COUNT = 1000

# PREDICTION_LAYER = True
# model_choice = 0

def closestDivisors(num):
    n = math.floor(math.sqrt(num))
    for i in range(n, 0, -1):
        if (num) % i == 0:
            res = [i, (num) // i]
            break
    return res

def model_MobileNetV2():
    image_model = tf.keras.applications.MobileNetV2(include_top=True, weights='imagenet', pooling='avg')
    image_model.summary()
    return image_model

def model_VGG16():
    image_model = tf.keras.applications.VGG16(include_top=True, weights='imagenet', pooling='avg')
    image_model.summary()
    return image_model

def model_VGG19():
    image_model = tf.keras.applications.VGG19(include_top=True, weights='imagenet', pooling='avg')
    image_model.summary()
    return image_model

def model_Xception():
    image_model = tf.keras.applications.Xception(include_top=True, weights='imagenet', pooling='avg')
    image_model.summary()
    return image_model

def model_DenseNet121():
    image_model =  tf.keras.applications.DenseNet121(include_top=True, weights='imagenet', pooling='avg')
    image_model.summary()
    return image_model

def model_DenseNet169():
    image_model =  tf.keras.applications.DenseNet169(include_top=True, weights='imagenet', pooling='avg')
    image_model.summary()
    return image_model

def model_DenseNet201():
    image_model =  tf.keras.applications.DenseNet201(include_top=True, weights='imagenet', pooling='avg')
    image_model.summary()
    return image_model

def model_InceptionV3():
    image_model = tf.keras.applications.InceptionV3(include_top=True, weights='imagenet', pooling='avg')
    image_model.summary()
    return image_model

def model_ResNet50():
    image_model = tf.keras.applications.ResNet50(include_top=True, weights='imagenet', pooling='avg')
    image_model.summary()
    return image_model

def model_ResNet101():
    image_model = tf.keras.applications.ResNet101(include_top=True, weights='imagenet', pooling='avg')
    image_model.summary()
    return image_model

def model_ResNet152():
    image_model = tf.keras.applications.ResNet152(include_top=True, weights='imagenet', pooling='avg')
    image_model.summary()
    return image_model

def model_ResNet50V2():
    image_model = tf.keras.applications.ResNet50V2(include_top=True, weights='imagenet', pooling='avg')
    image_model.summary()
    return image_model

def model_ResNet101V2():
    image_model = tf.keras.applications.ResNet101V2(include_top=True, weights='imagenet', pooling='avg')
    image_model.summary()
    return image_model

def model_ResNet152V2():
    image_model = tf.keras.applications.ResNet152V2(include_top=True, weights='imagenet', pooling='avg')
    image_model.summary()
    return image_model

def model_InceptionResNetV2():
    image_model = tf.keras.applications.InceptionResNetV2(include_top=True, weights='imagenet', pooling='avg')
    image_model.summary()
    return image_model

def model_NASNetLarge():
    image_model = tf.keras.applications.NASNetLarge(include_top=True, weights='imagenet', pooling='avg') 
    image_model.summary()
    return image_model

def model_NASNetMobile():
    image_model = tf.keras.applications.NASNetMobile(include_top=True, weights='imagenet', pooling='avg')
    image_model.summary()
    return image_model

def model_H5_model(model_path):
    image_model = tf.keras.models.load_model(model_path)
    image_model.summary()
    return image_model

# map the inputs to the function blocks
options = {0 : model_VGG16,
           1 : model_VGG19,
           2 : model_Xception,
           3 : model_DenseNet121,
           4 : model_DenseNet169,
           5 : model_DenseNet201,
           6 : model_MobileNetV2, 
           7 : model_InceptionV3,
           8 : model_InceptionResNetV2,
           9 : model_ResNet50,  
           10 : model_ResNet50V2,  
           11 : model_ResNet101,
           12 : model_ResNet101V2,
           13 : model_ResNet152,
           14 : model_ResNet152V2,
           15 : model_NASNetLarge,
           16 : model_NASNetMobile,
           17 : model_H5_model
}

def init(choice, layers_to_see, model_path):
    start = time.time()
    if model_path == "None":
        image_model = options[choice]()
    else:
        image_model = options[choice](model_path) #TODO check it out  #tf.keras.applications.MobileNetV2(include_top=True, weights='imagenet', pooling='avg')
    layers_indices = []
    for index, layer in enumerate(image_model.layers):
        test = any(n in layer.name for n in layers_to_see)        # TODO check  cu bogdan 
        if test:
            layers_indices.append(index)
    outputs = [image_model.layers[i].output for i in layers_indices]
    image_model = tf.keras.Model(inputs=image_model.inputs, outputs=outputs)
    end = time.time()
    result = end-start
    print("Initialization time for " + str(choice) +  " " + str(result))
    return image_model, layers_indices

def predict(index, img_bytes, image_model, layers_indices):
    data = {}
    if True:
        img = expand_dims(img_bytes, axis=0)
        feature_maps = image_model.predict(img)

        # When running for the first time, send also the labels in the order of the model (not sorted by probabilites).
        if index == 0:
            try:
                # Ordered of probability
                preds_top = tf.keras.applications.imagenet_utils.decode_predictions(feature_maps[-1], top=PREDICTIONS_COUNT)

                # feature_maps[-1] has the same values as preds_top[0]
                # Form the labels array with the order from feature_maps
                labels = [''] * len(preds_top)
                for (_, class_name, value) in preds_top[0]:
                    labels[np.where(feature_maps[-1][0] == value)[0][0]] = class_name
                data['labels'] = {f"{i}_{label}": [[42]] for (i, label) in enumerate(labels)}
            except Exception as e:
                print("WARN: Cannot extract labels for model")
                data['labels'] = {f"{i}": [[42]] for i in range(len(feature_maps[-1]))}
        
        startExtraction = time.time()
        layer_counter = -1
        for fmap in feature_maps:  # of each layer
            feature_map = {}
            layer_counter = layer_counter + 1
            ix = 1
            layer_name = image_model.layers[layers_indices[layer_counter]].name
            if 'fc' in layer_name:
                fmap = fmap - fmap.min()
                if fmap.max() != 0:
                    fmap = fmap / fmap.max() * 255.0
                feature_map[f'fmap_{layer_name}'] = fmap.astype(np.uint8).tolist()
            elif 'pred' in layer_name:
                fmap = fmap - fmap.min()
                if fmap.max() != 0:
                    fmap = fmap/fmap.max() * 255.0
                fmap = fmap.astype(np.uint8)
                name_pred = 'fmap_{}'.format(layer_name)   
                feature_map[name_pred] = fmap.astype(np.uint8).tolist()
            elif 'conv' in layer_name:
                if isinstance(image_model.layers[layers_indices[layer_counter]].output_shape[0], Iterable):
                    x, z = closestDivisors(
                        (image_model.layers[layers_indices[layer_counter]].output_shape[0])[3])
                else:
                    x, z = closestDivisors(
                        image_model.layers[layers_indices[layer_counter]].output_shape[3])
                for i in range(x):
                    for j in range(z):
                        imageArray = fmap[0, :, :, ix-1]  # images  ndarray ix-1
                        imageArray = imageArray - imageArray.min()
                        if imageArray.max() != 0:
                            imageArray = imageArray/imageArray.max() * 255.0
                        feature_map[f'fmap_{layer_name}_{i:02d}{j:02d}'] = imageArray.astype(np.uint8).tolist()
                        ix += 1
            data[str(image_model.layers[layers_indices[layer_counter]].name)] = feature_map
        endExtraction = time.time()
        print(f"Extracting features time for thread {index} - {endExtraction-startExtraction} - {datetime.datetime.now().strftime('%H:%M:%S')}")    
        start = time.time()
        elem = msgpack.packb(data) 
        # print(len(elem))
        end = time.time()
        print(f"Compresion time for thread {index} -  {end-start} - {datetime.datetime.now().strftime('%H:%M:%S')}")
        return elem


async def predict_and_write(index, image, image_model, layers_indices, writer):
    ''' Make the prediction and write it to the writer. '''
    if True: #try:
        nn_output = predict(index, image, image_model, layers_indices)
        writer.write(pack("i",len(nn_output)))
        writer.write(nn_output)
        await writer.drain()
        print(f"Job sent sucessfully {index} - {datetime.datetime.now().strftime('%H:%M:%S')}")



async def worker_task(index, image_queue, semaphore, image_model, layers_indices):
    ''' Handle prediction communication. '''

    async def work_thread(reader, writer): 
        while True:
            await reader.readline()
            semaphore.release()
            image = image_queue.get()
            await predict_and_write(index, image, image_model, layers_indices, writer)

    work = await asyncio.start_server(work_thread, HOST, PORT + index)
    await work.serve_forever()


def worker_thread(index, image_queue, semaphore, model_choice, layers_to_see, model_path):
    ''' Runs a thread doing predictions and communication with C# via asyncio. '''
    loop = asyncio.new_event_loop()
    image_model, layers_indices = init(model_choice, layers_to_see, model_path)   
    loop.create_task(worker_task(index, image_queue, semaphore, image_model, layers_indices))
    loop.run_forever()


async def server_task():
    ''' Handles communication with C#. '''

    async def server_work(reader, writer):
        ''' Handle main communication with C#. '''
        while True:
            # print("wait for server work")
            # Wait for an instruction from C# and act upon.
            # TODO: handle disconnects (they might throw some exceptions around read).
            recv = await reader.readline()
            # print(recv)
            try:
                data = recv.decode('utf-8').split()
            except (UnicodeDecodeError, AttributeError):
                print(f"Invalid data received from C#: {recv}")
                continue
            
            if data[0] == "start":
                num_threads = int(data[1])
                model_choice = int(data[2])
                image_size = int(data[3])
                model_path = data[4]
                visualization_type = int(data[5])
                video_path = data[6]
                frame_rate = int(data[7])
                layers_to_see = []
                layers_to_see.append("pred")
                for layers in range(8, len(data)):
                    layers_to_see.append(data[layers])
                # Start the cam worker.
                image_queue = Queue(num_threads)
                semaphore = Semaphore(0)
                Process(target=camera_worker, args=[image_queue, semaphore,image_size, visualization_type, video_path, frame_rate]).start() 
                # Make the initial prediction.
                image_model, layers_indices = init(model_choice, layers_to_see, model_path)
                semaphore.release()
                image = image_queue.get()
                # Start the prediction workers.
                for index in range(num_threads):
                    Process(target=worker_thread, args=(index + 1, image_queue, semaphore,model_choice, layers_to_see, model_path)).start()

                await predict_and_write(0, image, image_model, layers_indices, writer)

    
    server = await asyncio.start_server(server_work, HOST, PORT)
    print("Server started")
    await server.serve_forever()



def camera_worker(image_queue, semaphore, IMAGE_SIZE, visualization_type, video_path, frame_rate):
    ''' Captures camera and fills the queue with work for the prediction threads. '''
    print("start capturing ")

    if(visualization_type == 1):
        video_source = cv2.VideoCapture(video_path)      
        video_source.set(cv2.CAP_PROP_FPS, frame_rate)   
    else:
        video_source = cv2.VideoCapture(0)
    while True:
        semaphore.acquire()
        
        ret, frame = video_source.read()

        if not ret:
            raise Exception("Could not read camera")

        frame = cv2.resize(frame, (IMAGE_SIZE, IMAGE_SIZE), interpolation=cv2.INTER_AREA)
        image_queue.put_nowait(frame)
        # time.sleep(1)


def main(choice):
    ''' Script entry-point. ''' 
    global model_choice 
    model_choice = int(choice)
    loop = asyncio.get_event_loop()
    loop.create_task(server_task())
    loop.run_forever()
    sys.exit(0)


if __name__ == '__main__':

    main(0)





