import { Dispatch, SetStateAction, useEffect, useState } from "react";
import axios from "axios";
import { API_URL } from "./constants.ts";
import { Rating } from "./types/rating.ts";

interface Props {
  setIsModalOpen: Dispatch<SetStateAction<boolean>>;
}
export const Ratings = ({ setIsModalOpen }: Props) => {
  const [ratings, setRatings] = useState<Rating[]>([]);
  const [currentPage, setCurrentPage] = useState(0);
  useEffect(() => {
    axios
      .get(`${API_URL}/ratings`, {
        params: { page: currentPage, count: 5 },
        headers: { Authorization: "Bearer " + localStorage.getItem("token") },
      })
      .then((resp) => {
        console.log(resp.data.data);
        setRatings(resp.data.data);
      });
  }, [currentPage]);
  return (
    <div
      className="z-50 fixed flex items-center justify-center"
      style={{ backgroundColor: "white", minHeight: "30vh" }}
    >
      <div className="p-4 rounded shadow-lg w-80">
        <h2 className="text-lg font-bold mb-2">Рейтинги</h2>
        <ul>
          {ratings &&
            ratings.map((rating, index) => (
              <li key={index}>
                {rating.username} {rating.rating}
              </li>
            ))}
        </ul>
        <button onClick={() => setCurrentPage(currentPage + 1)}>Далее</button>
        <button onClick={() => setCurrentPage(currentPage - 1)}>Назад</button>
        <button
          onClick={() => setIsModalOpen(false)}
          className="mt-4 p-2 bg-red-500 text-white rounded"
        >
          Закрыть
        </button>
      </div>
    </div>
  );
};
