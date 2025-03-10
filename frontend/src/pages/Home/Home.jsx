import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { getListGames, createGame } from './../../services/gameService';
import { toast } from 'react-toastify';
import './Home.css'
import GamePreview from './../../components/GamePreview/GamePreview';
import RatingModal from './../../components/RatingModal/Rating';
import CreateGameModal from './../../components/CreateGameModal/CreateGame';

const Home = () => {
  const limit = 5;
  const [offset, setOffset] = useState(0);
  const [games, setGames] = useState([]);
  const [loading, setLoading] = useState(false);
  const [hasMore, setHasMore] = useState(true);
  const [showRatingModal, setShowRatingModal] = useState(false);
  const [showCreateGameModal, setShowCreateGameModal] = useState(false);
  const navigate = useNavigate();

  const onRatingClose = () => setShowRatingModal(false);
  const onGameCreateClose = () => setShowCreateGameModal(false);

  const onGameCreate = (maxRating) => {
      createGame({ maxRating })
          .then((res) => {
            toast.success('Игра успешно создана!')
            onGameCreateClose();
            if (res) navigate(`/game/${res}`)
          })
          .catch((error) => toast.error(`Неизвестная ошибка во время создания игры: ${error.message || error}`));
  };

  const loadGames = () => {
    if (loading || !hasMore) return;
    setLoading(true);

    getListGames({ offset, limit })
      .then((res) => {
        if (res.length < limit) {
          setHasMore(false);
        }
        const handled = res.map(element => {
          element.roomId = element.id
          element.creationDate = new Date(element.creationDate).toLocaleString("ru-RU")
          return element
        });
        setGames((prevGames) => [...prevGames, ...handled]);
        setOffset((prevOffset) => prevOffset + limit);
      })
      .catch((error) => {
        toast.error(`Ошибка загрузки игр: ${error.message || error}`);
      })
      .finally(() => {
        setLoading(false);
      });
  };

  useEffect(() => {
    loadGames();
  }, []);

  useEffect(() => {
    const handleScroll = () => {
      if (loading || !hasMore) return;
      if (window.innerHeight + window.scrollY >= document.documentElement.offsetHeight - 100) {
        loadGames();
      }
    };

    window.addEventListener('scroll', handleScroll);
    return () => window.removeEventListener('scroll', handleScroll);
  }, [loading, hasMore]);

  return (
    <div className="main-container">
      <h1 className="title">Rock - Paper - Scissors</h1>
      <div className="control-panel">
        <button className="create-game-button" onClick={() => setShowCreateGameModal(true)}>Создать игру</button>
        <button className="create-game-button" onClick={() => setShowRatingModal(true)}>Мой рейтинг</button>
      </div>
      {games.map((game) => (
        <GamePreview
          key={game.roomId}
          username={game.username}
          creationDate={game.creationDate}
          roomId={game.roomId}
          status={game.status}
        />
      ))}

      {loading && <p>Загрузка...</p>}
      {(!loading && hasMore) && <button onClick={loadGames}>Загрузить еще</button>}

      {showRatingModal && (
          <RatingModal onClose={onRatingClose}/>
      )}

      {showCreateGameModal && (
          <CreateGameModal onCreate={onGameCreate} onClose={onGameCreateClose}/>
        )}
    </div>
  );
};

export default Home;