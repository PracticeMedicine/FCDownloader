using System;
using System.Reflection;
using AridityTeam.Base;

namespace FCDownloader;

static class Program {
    private static CommandLineUtil? commandLineUtil = null;
    static void HandleExc(Exception ex) {

    }

    static int Main(string[] args) {
        commandLineUtil = new CommandLineUtil(args);

        if (commandLineUtil.FindParm("/version")) {
            Console.WriteLine("Version: {0}", Assembly.GetEntryAssembly()?.GetName()?.Version?.ToString());
            return 0;
        } else if (commandLineUtil.FindParm("/install")) {

        } else if (commandLineUtil.FindParm("/update")) {
        
        } else if (commandLineUtil.FindParm("/help")) {
            Console.WriteLine(
                "Welcome to FCDownloader!\n" +
                "usage: fcdownloader [options]\n\n" +
                "/install - installs the mod\n" +
                "/update - updates the mod\n" +
                "/help - show this text\n" 
            );
        } else {
            RunInteractive();
        }

        return 0;
    }

    static void RunInteractive() {
        Console.Clear();
        int choice = 0;

        Console.WriteLine(
            "Welcome to FCDownloader!\n" +
            "[1] Install Fortress Connected\n" +
            "[2] Update Fortress Connected\n" +
            "[3] Exit\n"
        );

        int.TryParse(Console.ReadLine(), out choice);

        switch(choice) {
            case 1:
                break;
            case 2:
                break;
            case 3:
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Invalid choice! Do you want to return to the menu?");
                string? goBackChoice = Console.ReadLine();

                switch(goBackChoice) {
                    case "y":
                        RunInteractive();
                        break;
                    case "n":
                        Environment.Exit(0);
                        break;
                    default:
                        RunInteractive();
                        break;
                }

                break;
        }
    }
}
