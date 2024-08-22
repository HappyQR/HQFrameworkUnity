namespace HQFramework
{
    public interface IBinaryHelper
    {
        byte[] ToBytes(object obj);

        T ToObject<T>(byte[] bytes);

        object ToObject(byte[] bytes);
    }
}
