# Chess AI Project - ReadMe
![chess1](https://github.com/user-attachments/assets/09725026-a0ab-4dcd-a921-c3aa425ac543)

## Download

[Download Windows Version via Github Releases](https://github.com/IlyaKaikov/Chess-AI/releases/tag/v1.0.0)

## Overview

This Unity-based Chess AI project lets players compete against an AI or Practice (solo).

The AI employs advanced techniques like Minimax with Alpha-Beta pruning and positional evaluation for more challenging gameplay.

For C# code check out /Assets/Scripts

### Features

Game Modes: Practice (solo), Human vs AI (play as White/Black)

Chess Rules: Supports castling, En Passant, pawn promotion, checkmate, stalemate.

### How to Play

Select a game mode.

Drag pieces to valid positions.

Use right-click for marking moves or strategies.

## AI

Minimax Algorithm with Alpha-Beta pruning and iterative deepening

Move ordering evaluation for faster and more effiecient pruning

Positional evaluation for adaptive strategies

## Testing

Move generation validated through [Perft](https://www.chessprogramming.org/Perft_Results) testing for various positions and compared with the [Stockfish](https://www.chessprogramming.org/Stockfish) chess engine to ensure theoretical accuracy.

For test scripts check out /Assets/Scripts/Tests

## Interactive UI:

Move highlights, last move indicators, and marking tiles or moves

Input [FEN](https://www.chessprogramming.org/Forsyth-Edwards_Notation) strings and set game timers

## License

This project is licensed under the MIT License.

## Credits

Developed by Ilya Kaikov.

![chess2](https://github.com/user-attachments/assets/103a4190-4aa9-4ed9-9fa6-b389f6752307)
![chess3](https://github.com/user-attachments/assets/1055d2fb-e2dd-4a72-aa2f-908434efc552)
![chess4](https://github.com/user-attachments/assets/459fe76c-dd23-4284-bbbc-35a01abd4b34)
