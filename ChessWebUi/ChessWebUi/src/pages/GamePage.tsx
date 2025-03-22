import ChessboardComponent from "../components/ChessboardComponent";
import { useSignalR } from "../hooks/useSignalR";
import { makeMove } from "../services/chessService";
import {useEffect, useState} from "react";
import {Chess} from "chess.js";
import {Colors} from "../models/Colors.ts";

export default function GamePage() {
    //const [game, setGame] = useState(new Chess());
    const { connection, gameId, color, game, createGame } = useSignalR();
    const [localGame,setLocalGame] = useState(game);

    useEffect(() => {
        setLocalGame(game);
    }, [game])

    const handleMove = async (source: string, target: string, piece: any) => {

        // Optimistically update board
        const gameCopy = new Chess(localGame?.fen());
        const move = gameCopy.move({
            from: source,
            to: target,
            promotion: piece[1].toLowerCase() ?? "q"
        });

        if (move) {
            setLocalGame(gameCopy); // Update UI immediately
        }

        // Send move to server (non-blocking)
        makeMove(source, target, piece, connection, gameId);

        return true; // Always accept the move
    };

    return (
        <div>
            <button onClick={() => createGame(connection)}>Create Game</button>
            <ChessboardComponent position={localGame?.fen()} handleMove={handleMove} color={color}/>
        </div>
    );
}
