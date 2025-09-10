using Microsoft.AspNetCore.Mvc;

namespace SMS.Web.Controllers;
public enum AlertType { success, danger, warning, info }
public class BaseController : Controller
{
    // Store Alert in TempData Storage thus only accessible in next Request
    public void Alert(string message, AlertType type = AlertType.info)
    {
        TempData["Alert.Message"] = message;
        TempData["Alert.Type"] = type.ToString();
    }

    public void SendAlert(bool condition, string success, string failure)
    {
        if (condition)
        {
            Alert(success, AlertType.success);
        }
        else
        {
            Alert(failure, AlertType.danger);
        }
    }
}
