///For examples, see:
///https://thegraybook.vvvv.org/reference/extending/writing-nodes.html#examples

namespace TemporalSpace;

public static class Remap
{
    public static float RemapValue(float value, float min1, float max1, float min2, float max2)
    {
        return min2 + (value - min1) * (max2 - min1) / (max1 - min1);
    }
}