using ConsoleFileManager;
using ConsoleFileManager.Services;
using FileManager;
using FileManager.Services;

var fileManager = new FileManagerLogic(OSNavigator.Navigator, new ConsoleMessageService());
new ConsoleFileManagerLogic(fileManager).Start();