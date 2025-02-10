import { useContext, useEffect, useState } from "react";
import axios from "axios";
import { API_URL } from "./constants.ts";
import { GameRoom } from "./types/gameRoom.ts";
import { AuthContext } from "./authContext.ts";
import { useNavigate } from "react-router-dom";

export const GamesList = () => {
  const [currentPage, setCurrentPage] = useState(0);
  const [games, setGames] = useState<GameRoom[]>([]);
  useEffect(() => {
    axios
      .get(`${API_URL}/games`, { params: { page: currentPage, count: 5 } })
      .then((resp) => {
        setGames(resp.data);
      });
  }, [currentPage]);
  const { userId } = useContext(AuthContext);
  const navigate = useNavigate();

  const handleOnJounGame = (gameId: string) => {
    navigate("/main", { state: { gameId: gameId, userId: userId } });
  };

  return (
    <div>
      <h1>Games</h1>
      <div>
        {games.map((game) => (
          <div className={"flex flex-row mt-2"}>
            <div className={"ml-2"}>{game.id}</div>
            <div className={"ml-2"}>{game.dateTime}</div>
            <div className={"ml-2"}>{game.ownerName}</div>
            <button onClick={() => handleOnJounGame(game.id)}>Войти</button>
          </div>
        ))}
      </div>
      <button onClick={() => setCurrentPage(currentPage + 1)}>Далее</button>
      <button onClick={() => setCurrentPage(currentPage - 1)}>Назад</button>
      <footer>
        <button>Create Game</button>
        <button>Show Stats</button>
      </footer>
    </div>
  );
};
