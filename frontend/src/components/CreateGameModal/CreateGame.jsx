import React, { useState } from 'react';
import './CreateGame.css';

const CreateGameModal = ({ onCreate, onClose }) => {
  const [maxRating, setMaxRating] = useState('');

  const handleSubmit = (e) => {
    e.preventDefault();
    if (onCreate) {
      onCreate(maxRating);
    }
    onClose();
  };

  const handleClose = () => {
    onClose();
  }

  return (
    <div className="modal-overlay">
      <div className="game-modal-content">
        <h2 className="game-modal-form-title">Новая Игра</h2>
        <form onSubmit={handleSubmit} className="form-group-container">
          <div className="form-group">
            <label htmlFor="maxRating">Max Rating</label>
            <input
              type="number"
              id="maxRating"
              value={maxRating}
              onChange={(e) => setMaxRating(e.target.value)}
              required
            />
          </div>
          <div className="form-group-control-panel">
            <button onClick={handleClose}>Закрыть</button>
            <button type="submit">Создать</button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default CreateGameModal;