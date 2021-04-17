﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using NAudio.Wave;
using System.Threading;
namespace Simple_Music_Player
{

    class Program
    {
        static void Main(string[] args)
        {
            mainProgram(args);
        }
        static void mainProgram(string[] args)
        {
            
            string directory;
            Console.WriteLine("What do you want to do?");
            Console.WriteLine("[1] Play Directory");
            Console.WriteLine("[2] Play Directory Shuffled");
            Console.WriteLine("[3] Play a file");
            string selection = Console.ReadLine();
            switch (selection[0])
            {
                case '1':
                    Console.WriteLine("What directory has music files?");
                    directory = Console.ReadLine();
                    if (dirChk(directory, false) == "null")
                    {
                        Main(args);
                        return;
                    }
                    MusicData.queue = ProcessDirectory(directory);
                    playMusic(args);
                    break;
                case '2':
                    Console.WriteLine("What directory has music files?");
                    directory = Console.ReadLine();
                    if (dirChk(directory, false) == "null")
                    {
                        Main(args);
                        return;
                    }
                    MusicData.queue = (ProcessDirectory(directory)).OrderBy(x => Guid.NewGuid()).ToList();
                    playMusic(args);
                    break;
                case '3':
                    Console.WriteLine("What file do you want to play?");
                    directory = Console.ReadLine();
                    if (dirChk(directory, true) == "null")
                    {
                        Main(args);
                        return;
                    }
                    MusicData.queue.Add(directory);
                    playMusic(args);
                    break;
                default:
                    Console.WriteLine("Select a valid input.");
                    Main(args);
                    break;
            }
        }
        public static List<string> ProcessDirectory(string targetDirectory)
        {
            List<string> lst = new List<string>();
            dirChk(targetDirectory, false);
            // Process the list of files found in the directory.
            var fileEntries = Directory.GetFiles(targetDirectory, "*.mp3", SearchOption.AllDirectories).Union(Directory.GetFiles(targetDirectory, "*.flac", SearchOption.AllDirectories));
            foreach (string fileName in fileEntries)
                lst.Add(fileName);
            return lst;
        }
        static string dirChk(string dir, bool file)
        {
            switch (file)
            {
                case true:
                    if (!System.IO.File.Exists(dir))
                    {
                        Console.WriteLine("That is not a vaild file.");
                        return "null";
                    }
                    break;
                case false:
                    if (!Directory.Exists(dir))
                    {
                        Console.WriteLine("That is not a vaild directory.");
                        return "null";
                    }
                    break;
            }
               
            return dir;
        }
        public static void playMusic(string[] args)
        {
            using (var audioFile = new AudioFileReader(MusicData.queue[0]))
            using (var outputDevice = new WaveOut())
            {
                outputDevice.Init(audioFile);
                outputDevice.Play();
                TagLib.File file = TagLib.File.Create(MusicData.queue[0]);
                Console.Clear();
                Console.WriteLine("Title: {0}", file.Tag.Title);
                Console.WriteLine("Artist: {0}", file.Tag.Performers);
                Console.WriteLine("Album: {0}", file.Tag.Album);
                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(1000);
                    double ms = outputDevice.GetPosition() * 1000.0 / audioFile.WaveFormat.BitsPerSample / audioFile.WaveFormat.Channels * 8 / audioFile.WaveFormat.SampleRate;
            TimeSpan ts = TimeSpan.FromMilliseconds(ms);
                    RewriteLine(4, ts.ToString(@"hh\:mm\:ss"));
                }
                MusicData.queue.RemoveAt(0);
                if (MusicData.queue.Count() >= 1)
                {
                    playMusic(args);
                        return;
                }
            }
            mainProgram(args);
        }
        public static void RewriteLine(int lineNumber, String newText)
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, lineNumber - 1);
            Console.Write(newText); Console.WriteLine(new string(' ', Console.WindowWidth - newText.Length));
            Console.SetCursorPosition(0, currentLineCursor);
        }
    }
    static class MusicData
    {
        public static List<string> queue = new List<string>();
    }
 
}
