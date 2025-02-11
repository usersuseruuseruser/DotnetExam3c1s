import { useContext, useEffect, useState } from "react";
import axios from "axios";
import { API_URL } from "./constants.ts";
import { GameRoom } from "./types/gameRoom.ts";
import { AuthContext } from "./authContext.ts";
import { useNavigate } from "react-router-dom";
import { Ratings } from "./Ratings.tsx";
import { format, isBefore } from "date-fns";

export const GamesList = () => {
  const [currentPage, setCurrentPage] = useState(0);
  const [games, setGames] = useState<GameRoom[]>([]);
  const [gameAdded, setGameAdded] = useState(false);
  useEffect(() => {
    axios
      .get(`${API_URL}/all`, {
        params: { page: currentPage, count: 5 },
        headers: { Authorization: "Bearer " + localStorage.getItem("token") },
      })
      .then((resp) => {
        setGames(resp.data.data);
      });
  }, [currentPage, gameAdded]);
  const { userId } = useContext(AuthContext);
  const navigate = useNavigate();
  const [isModalOpen, setIsModalOpen] = useState(false);

  const handleOnJounGame = (gameId: string) => {
    navigate("/main", { state: { gameId: gameId, userId: userId } });
  };

  return (
    <div>
      {isModalOpen && <Ratings setIsModalOpen={setIsModalOpen} />}
      <h1>Games</h1>
      <div>
        {games &&
          games
            .sort((a, b) => {
              return isBefore(new Date(a.date), new Date(b.date))
                ? a.status < b.status
                  ? -1
                  : 1
                : -1;
            })
            .map((game, index) => (
              <div key={index} className={"flex flex-row mt-2"}>
                <div className={"ml-2"}>
                  {format(new Date(game.date), "HH:mm")}
                </div>
                <div className={"ml-5"}>{game.ownerName}</div>
                <button
                  className={"m-2"}
                  onClick={() => handleOnJounGame(game.gameId)}
                >
                  Войти
                </button>
              </div>
            ))}
      </div>
      <button onClick={() => setCurrentPage(currentPage + 1)}>Далее</button>
      <button onClick={() => setCurrentPage(currentPage - 1)}>Назад</button>

      <footer>
        <button
          onClick={() => {
            axios
              .post(
                `${API_URL}/game/create`,
                { maxRating: 100 },
                {
                  headers: {
                    Authorization: "Bearer " + localStorage.getItem("token"),
                  },
                },
              )
              .then(() => {
                setGameAdded(!gameAdded);
              });
          }}
        >
          Create Game
        </button>
        <button
          onClick={() => {
            setIsModalOpen(true);
          }}
        >
          Show Stats
        </button>
      </footer>
    </div>
  );
};
