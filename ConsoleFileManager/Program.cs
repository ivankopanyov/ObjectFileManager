using ConsoleFileManager;
using ConsoleFileManager.Services;
using FileManager.Services;

new ConsoleFileManagerLogic(OSNavigator.Navigator, new ConsoleMessageService()).Start();