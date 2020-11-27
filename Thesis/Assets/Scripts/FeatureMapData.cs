public class FeatureMapData
{
    private int height;
    private int width;
    private int coordX;
    private int coordY;
    private int coordZ;
    private string name;
    private int scale;
    public FeatureMapData (int height, int width, int coordX, int coordY, int coordZ, string name){
       this.height = height;
       this.width = width;
       this.coordX = coordX;
       this.coordY = coordY;
       this.coordZ = coordZ;
       this.name = name;
   }
   public FeatureMapData(){ }
   public int getHeight()
    {
        return this.height;
    }
    public void setHeight(int height)
    {
        this.height = height;
    }
    public int getWidth()
    {
        return this.width;
    }
    public void setWidth(int width)
    {
        this.width = width;
    }
    public int getCoordX()
    {
        return this.coordX;
    }
    public void setCoordX(int coordX)
    {
        this.coordX = coordX;
    }
    public int getCoordY()
    {
        return this.coordY;
    }
    public void setCoordY(int coordY)
    {
        this.coordY = coordY;
    }
    public int getCoordZ()
    {
        return this.coordZ;
    }
    public void setCoordZ(int coordZ)
    {
        this.coordZ = coordZ;
    }   
    public string getName()
    {
        return this.name;
    }
    public void setName(string name)
    {
        this.name = name;
    }

    public int getCoordScale()
    {
        return this.scale;
    }
    public void setCoordScale(int scale)
    {
        if (scale < 7)
        {
            this.scale = 1;
        }
        else {
            this.scale = scale - 1;
        }
    }
}
