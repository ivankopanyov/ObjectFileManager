using System.Windows;

namespace ObjectFileManager.Views;

/// <summary>Главное окно приложения.</summary>
public partial class MainWindow : Window
{
    /// <summary>Инициализация главного окна приложения.</summary>
    public MainWindow()
    {
        InitializeComponent();
        Language = System.Windows.Markup.XmlLanguage.GetLanguage(System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag);
    }
}
