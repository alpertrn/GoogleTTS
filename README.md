# GoogleTTS

A simple .NET console app that generates Turkish text-to-speech MP3 files in bulk, from category-based text lists, using Google Cloud Text-to-Speech.

## What it does

`GoogleTTS/Program.cs` defines a dictionary called `categoryTexts`, where texts are grouped by category number. For each category, the program creates a folder named `01`, `02`, ... and synthesizes each text in that category with Google Cloud TTS, saving the results as `001.mp3`, `002.mp3`, ...

Generation is **incremental**: re-running the program skips MP3s that already exist in a folder and only generates the missing ones. This means that when you add new sentences to a category, only the new ones get synthesized.

Texts can be written in two ways:
- **Plain text:** a normal sentence, e.g. `"Kapak açılıyor!"`.
- **SSML:** any text starting with `<speak>` is treated as SSML, so tags like `<prosody rate='95%' pitch='-1st'>`, `<break time='400ms'/>`, and `<emphasis level='moderate'>...</emphasis>` can be used to control speech rate, pitch, pauses and emphasis. This is demonstrated in category 2.

## Setup

1. Make sure the **.NET 10 SDK** is installed (`dotnet --list-sdks`).
2. Enable the **Text-to-Speech API** in a Google Cloud project and create a service account key.
3. Save the downloaded JSON key as `GoogleTTS/service_account.json` (this file is excluded via `.gitignore` - each developer uses their own key).
4. Restore packages and run:
   ```
   cd GoogleTTS
   dotnet restore
   dotnet run
   ```
5. Once it finishes, you'll find the generated MP3s under folders like `GoogleTTS/01/`, `GoogleTTS/02/`, etc.

## Adding your own categories/texts

Just add a new entry to the `categoryTexts` dictionary in `Program.cs`:

```csharp
{3, new List<string> {
    "First sentence",
    "Second sentence",
    "<speak><prosody rate='95%'>An SSML example</prosody></speak>"
}},
```

To change the voice settings (language, voice name, gender), edit the `voice` object; to change the output format, edit the `audioConfig` object. See the [list of available Google Cloud TTS voices](https://cloud.google.com/text-to-speech/docs/voices).

## Branch structure

- `main`: the simplified/example version described in this README, meant for anyone browsing the repo from the outside.
- `development`: the branch where the real project content (all categories and texts) is developed.
