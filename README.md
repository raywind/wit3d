# Wit3D

Proof-of-concept Unity application using arbitrary voice commands to control the placement of objects on the screen.

Copyright (c) 2016 Aaron Faucher
MIT License

## Demo

View a demo of Wit3D in action here:
https://www.youtube.com/watch?v=QRx37mivPiQ

Augmented reality proof-of-concept demonstration:
https://www.youtube.com/watch?v=34RkapT_IXQ

## Usage

1. Download and open the Unity project folder.
2. Open the `02_More Objects` scene.
3. Run the scene.
4. Voice a command to the scene by:
  - Press the spacebar
  - Voice your command i.e. "Put the box on the chair."
  - Release the spacebar

## How it works

While the spacebar is being held, Unity records a sample of your voice from the microphone. On release of the spacebar, it streams this file to Wit.ai, where the command is processed and interpreted. Wit.ai returns a JSON response containing a 'subject' [i.e. box] and 'destination' [i.e. chair] from the command. This JSON response is parsed by Unity, which identifies the GameObjects by name in the scene.

## Extensibility

While this is a relatively simple example, building a bridge between Unity and Wit.ai allows developers to leverage the powerful Wit.ai NLP engine for their AR/VR applications. End users can now interact with AR/VR scenes using their voice, with Wit.ai taking on the role of interpreting these commands.
