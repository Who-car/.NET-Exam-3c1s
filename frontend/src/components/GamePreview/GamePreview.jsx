import React from 'react';
import './GamePreview.css';
import { Link } from 'react-router-dom';

const GamePreview = ({ author, creationDate, roomId, status }) => {
  return (
    <div className="game-preview">
      <div className="game-info">
        <p>
          <strong>Автор:</strong> {author}
        </p>
        <p>
          <strong>Создана:</strong> {creationDate}
        </p>
        <p>
          <strong>ID:</strong> {roomId}
        </p>
        <p>
          <strong>Статус:</strong> {status}
        </p>
      </div>
      <div className="game-action">
        <Link to={`/game/${roomId}`}>
          <button className="join-button">Присоединиться</button>
        </Link>
      </div>
    </div>
  );
};

export default GamePreview;