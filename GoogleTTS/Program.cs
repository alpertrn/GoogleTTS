using System;
using System.Collections.Generic;
using System.IO;
using Google.Cloud.TextToSpeech.V1;

// =====================================================================
// GoogleTTS - Batch MP3 generator using Google Cloud Text-to-Speech
// =====================================================================
// This project generates Turkish text-to-speech MP3 files in bulk from
// a set of category-based text lists.
//
// Below is a SMALL example dataset showing how the system works (in a
// real deployment you'd typically have dozens of categories and
// hundreds of lines). See README.md for how to add your own
// categories/texts.
// =====================================================================

class Program
{
    static void Main(string[] args)
    {
        // Google Cloud credentials file: place your own Google Cloud
        // service account key at the project root as "service_account.json".
        // (This file is excluded via .gitignore - every developer uses
        // their own key.)
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "service_account.json");

        // Create the TTS client
        var client = TextToSpeechClient.Create();

        // 1) Define texts grouped by category.
        //    Key:   category number -> output folder name (01, 02, ...)
        //    Value: list of sentences/SSML for that category
        Dictionary<int, List<string>> categoryTexts = new Dictionary<int, List<string>>()
        {
            // Example 1: plain-text category
            {1, new List<string> {
                "Harika nasılsın?!",
                "Günün nasıl geçiyor?."
            }},

            // Example 2: SSML category - controls speech rate, pitch,
            // pauses (break) and emphasis for a more natural delivery
            {2, new List<string> {
                "<speak><prosody rate='95%' pitch='-1st'>Küçük bir mola vermeye ne dersin <break time='400ms'/> Bir bardak <emphasis level='moderate'>su içmek</emphasis> seni çok daha zinde hissettirir</prosody></speak>",
                "<speak><prosody rate='95%' pitch='+1st'>Vücudunun yüzde yetmişinin <emphasis level='strong'>su</emphasis> olduğunu biliyor muydun <break time='250ms'/> Onu susuz bırakma</prosody></speak>"
            }}

            // To add a new category, add a line like this:
            // {3, new List<string> { "First sentence", "Second sentence" }},
        };

        // 2) Voice settings (feel free to change these)
        var voice = new VoiceSelectionParams
        {
            LanguageCode = "tr-TR",
            Name = "tr-TR-Wavenet-D",
            SsmlGender = SsmlVoiceGender.Female
        };

        var audioConfig = new AudioConfig
        {
            AudioEncoding = AudioEncoding.Mp3
        };

        // 3) Create folders and generate MP3s (incremental: re-running the
        //    program skips MP3s that already exist in a folder and only
        //    generates the missing ones)
        foreach (var category in categoryTexts)
        {
            int catNumber = category.Key;
            var texts = category.Value;

            // Folder name is zero-padded to 2 digits: 01, 02, ...
            string folderName = $"{catNumber:D2}";
            Directory.CreateDirectory(folderName);

            // Count how many MP3s already exist
            int existingFiles = Directory.GetFiles(folderName, "*.mp3").Length;

            for (int i = 0; i < texts.Count; i++)
            {
                // Skip if an MP3 for this text already exists
                if (i < existingFiles)
                {
                    Console.WriteLine($"Category {catNumber:D2} - MP3 already exists: {(i + 1):D3}.mp3");
                    continue;
                }

                string text = texts[i];

                // If the text starts with "<speak>", treat it as SSML,
                // otherwise treat it as plain text
                SynthesisInput input = text.TrimStart().Contains("<speak>")
                    ? new SynthesisInput { Ssml = text }
                    : new SynthesisInput { Text = text };

                var response = client.SynthesizeSpeech(input, voice, audioConfig);

                // File name is zero-padded to 3 digits: 001.mp3, 002.mp3, ...
                string fileName = Path.Combine(folderName, $"{(i + 1):D3}.mp3");
                using (var output = File.Create(fileName))
                {
                    response.AudioContent.WriteTo(output);
                }

                Console.WriteLine($"Category {catNumber:D2} - New MP3 created: {fileName}");
            }
        }

        Console.WriteLine("All new audio files were created successfully!");
    }
}
