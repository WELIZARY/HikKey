//CardInfo.cs
namespace HikKey;

public class CardInfo
{
    public string CardType { get; set; }
    public string SerialNumber { get; set; }
    public int SerialNumberLength { get; set; }
    public string SelectVerifyCode { get; set; }
    public int SelectVerifyCodeLength { get; set; }
}