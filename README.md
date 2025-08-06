# life

![Godot Engine](https://img.shields.io/badge/GODOT-%23FFFFFF.svg?style=for-the-badge&logo=godot-engine) 
![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white)
![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)

## Introduction
This project aims to recreate at a small scale the cellular automaton The Game of Life, first designed by John Conway in 1970. Here we are using the Godot game engine programmed in C#. A version of the game true to the original rules can be played [here](https://playgameoflife.com/).

## Description
This version of The Game of Life runs in a more limited space than the original. The available grid for this game is 20x20 cells. Also, in addition to the standard life that follows Conways ruleset, there are two additional varieties of life included here. Those are Solitary and Social, and here the original variety is referred to as Average.

## How to Play
![](https://raw.githubusercontent.com/rjennett/life/refs/heads/main/media/life-demo.gif)

Select which type of life to place on the board, then press play. Pause any time to add more life. 

Each type of life follows different rules:
| Life Type | Lives | Dies |
| --- | --- | --- |
| Average | 2 or 3 live neighbors | Fewer than 2 or greater than 3 live neighbors |
| Solitary | 1 or 2 live neighbors | Fewer than 1 or greater than 2 live neighbors |
| Social | 3 or 4 live neighbors | Fewer than 3 or greater than 4 live neighbors |

## Attributions

This project uses assets from the free license of the lovely Sprout Lands asset pack created and generously shared by [Cup Nooble](https://cupnooble.itch.io/).