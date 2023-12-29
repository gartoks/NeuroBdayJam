namespace NeuroBdayJam.Util.Math.Random;
public interface INoiseGenerator {

    double Generate(double x);
    double Generate(double x, double y);
    double Generate(double x, double y, double z);
    //double Generate(double x, double y, double z, double w);

    /*uint GenerateUint(int x);


    int GenerateInt(int x);


    int GenerateInt(int x, int minValue, int maxValue);


    int GenerateInt(int x, int maxValue);


    double GenerateDouble(int x);


    double GenerateDouble(int x, double max);


    double GenerateDouble(int x, double min, double max);


    float GenerateFloat(int x);


    float GenerateFloat(int x, float max);


    float GenerateFloat(int x, float min, float max);


    bool GenerateBool(int x);


    bool GenerateChance(int x, double chanceForTrue);


    double GenerateAngle(int x);

    uint GenerateUint2D(int x, int y);

    int GenerateInt2D(int x, int y);

    int GenerateInt2D(int x, int y, int minValue, int maxValue);

    int GenerateInt2D(int x, int y, int maxValue);

    double GenerateDouble2D(int x, int y);

    double GenerateDouble2D(int x, int y, double max);

    double GenerateDouble2D(int x, int y, double min, double max);

    float GenerateFloat2D(int x, int y);

    float GenerateFloat2D(int x, int y, float max);

    float GenerateFloat2D(int x, int y, float min, float max);

    bool GenerateBool2D(int x, int y);

    bool GenerateChance2D(int x, int y, double chanceForTrue);

    double GenerateAngle2D(int x, int y);

    uint GenerateUint3D(int x, int y, int z);

    int GenerateInt3D(int x, int y, int z);

    int GenerateInt3D(int x, int y, int z, int minValue, int maxValue);

    int GenerateInt3D(int x, int y, int z, int maxValue);

    double GenerateDouble3D(int x, int y, int z);

    double GenerateDouble3D(int x, int y, int z, double max);

    double GenerateDouble3D(int x, int y, int z, double min, double max);

    float GenerateFloat3D(int x, int y, int z);

    float GenerateFloat3D(int x, int y, int z, float max);

    float GenerateFloat3D(int x, int y, int z, float min, float max);

    bool GenerateBool3D(int x, int y, int z);

    bool GenerateChance3D(int x, int y, int z, double chanceForTrue);

    double GenerateAngle3D(int x, int y, int z);*/
}