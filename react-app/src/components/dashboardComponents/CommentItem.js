import { React, useState, useEffect } from 'react'
import { Link } from 'react-router-dom';

export default function CommentItem({ id, rating, body }) {
  const fullStars = Math.round(rating * 2) / 2;
  const hasHalfStar = fullStars % 1 !== 0;

  const renderStars = () => {
    const starArray = [];

    for (let i = 1; i <= 5; i++) {
      if (i <= fullStars) {
        starArray.push(<i key={i} className="bi bi-star-fill text-warning"></i>);
      } else if (hasHalfStar && i === Math.ceil(fullStars)) {
        starArray.push(<i key={i} className="bi bi-star-half text-warning"></i>);
      } else {
        starArray.push(<i key={i} className="bi bi-star text-warning"></i>);
      }
    }
    return starArray;
  }

  return (
    <Link to={"/Recipes/Detail/" + id} className='text-decoration-none hoverable' style={{ width: "100%", color: "black", background: "#f7f7f7" }}>
      <div className='d-flex m-3 overflow-hidden'>
        <div className='d-flex flex-column justify-content-between w-100'>
          <div>
            <div className='d-flex justify-content-between w-100'>
              <p className="me-3" style={{ color: "black !important" }}>Recipe name</p>
              <div className='starContainer text-warning d-flex'>
                {renderStars()}
              </div>
            </div>
            <p className='m-0'> {body} </p>
          </div>
        </div>
      </div>
    </Link >
  );
}