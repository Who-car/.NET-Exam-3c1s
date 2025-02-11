import React, { useState, useEffect } from 'react';
import { getRating } from './../../services/gameService';
import './Rating.css';

const RatingModal = ({ onClose }) => {
  const [rating, setRating] = useState(null);

  useEffect(() => {
    getRating()
      .then((res) => {
        setRating(res);
      });
  }, []);

  return (
    <div className="modal-overlay">
      <div className="modal-content">
        <h2>Ваш рейтинг: {rating}</h2>
        <button onClick={onClose}>ОК</button>
      </div>
    </div>
  );
};

export default RatingModal;