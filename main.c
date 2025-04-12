// filepath: /workspaces/CMusic/main.c
#include <SDL2/SDL.h>
#include <stdio.h>

int main(int argc, char *argv[]) {
    if (argc < 2) {
        printf("Usage: %s <audio_file.wav>\n", argv[0]);
        return 1;
    }

    if (SDL_Init(SDL_INIT_AUDIO) < 0) {
        printf("SDL could not initialize! SDL_Error: %s\n", SDL_GetError());
        return 1;
    }

    SDL_AudioSpec wavSpec;
    Uint32 wavLength;
    Uint8 *wavBuffer;

    if (SDL_LoadWAV(argv[1], &wavSpec, &wavBuffer, &wavLength) == NULL) {
        printf("Failed to load WAV file! SDL_Error: %s\n", SDL_GetError());
        SDL_Quit();
        return 1;
    }

    SDL_AudioDeviceID deviceId = SDL_OpenAudioDevice(NULL, 0, &wavSpec, NULL, 0);
    if (deviceId == 0) {
        printf("Failed to open audio device! SDL_Error: %s\n", SDL_GetError());
        SDL_FreeWAV(wavBuffer);
        SDL_Quit();
        return 1;
    }

    SDL_QueueAudio(deviceId, wavBuffer, wavLength);
    SDL_PauseAudioDevice(deviceId, 0);

    printf("Playing audio... Press Enter to quit.\n");
    getchar();

    SDL_CloseAudioDevice(deviceId);
    SDL_FreeWAV(wavBuffer);
    SDL_Quit();

    return 0;
}