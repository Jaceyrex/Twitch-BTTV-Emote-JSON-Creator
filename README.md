# Twitch+BTTV Emote JSON Creator

Console application used to gather all Global Twitch emotes and all Global BTTV (Better Twitch TV) emotes, saves the relevant information for planned usage and concatonates them all into a list with converted values and (ideally) fixed URLs.

Currently asks yes / no questions regarding creating a new JSON for emote data, downloads the emote images (Still need to put this into a question), then prompts for an answer as to whether the emotes should be verified and outputs a list of the emotes which are missing for whatever reason (likely due to issues with the downloading part or potentially being saved with illegal characters in the name, although this should be resolved by now.

Channel based emotes currently being done through a separate class with sensitive information not being uploaded to the repository for security's sake.

After all emotes have been gathered, a JSON will then be saved to the downloads folder.

FFZ is planned but not yet implemented.
