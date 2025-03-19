import {useState} from "react";
import {Chess} from "chess.js";
import {Chessboard} from "react-chessboard";

export default function GamePage() {
    const [game, setGame] = useState(new Chess());

    const handleMove = (source: string, target: string, piece: any) => {
        // @ts-ignore
        const gameCopy = new Chess(game.fen());
        const move = gameCopy.move({
            from: source,
            to: target,
            promotion: piece[1].toLowerCase() ?? "q"
        });
        setGame(gameCopy);

        return move !== null;
    };

    return (
        <div style={{ width: "500px", height: "500px" }}>
            <Chessboard
                position={game.fen()}
                onPieceDrop={(sourceSquare, targetSquare, piece) =>
                    handleMove(sourceSquare, targetSquare,piece)
                }
                animationDuration = {0}
            />
        </div>
    );
}
