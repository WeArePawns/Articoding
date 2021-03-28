
public interface ILaserReceiver
{
    // First time received
    void OnLaserReceived();
    // While receiving
    void OnLaserReceiving();
    // When laser is not being receivede anymore
    void OnLaserLost();
}
