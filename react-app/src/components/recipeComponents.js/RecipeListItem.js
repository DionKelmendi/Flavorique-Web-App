import { React, useState, useEffect } from 'react'
import { Link } from 'react-router-dom';

export default function RecipeListItem({ id, src, title, reviews, rating }) {

  const fullStars = Math.round(rating * 2) / 2;
  const hasHalfStar = fullStars % 1 !== 0;

  console.log(fullStars);
  console.log(hasHalfStar);

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
    <Link to={"/recipe/" + id} className='text-decoration-none mt-5' style={{ width: "25%", color: "black" }}>
      <div className='d-flex flex-column justify-content-center align-items-center text-center m-0'>
        <img width={250} height={250} src={src} />
        <div className='d-flex flex-column justify-content-between align-items-center'>
          <div>
            <div className='starContainer text-warning mt-2'>
              {renderStars()}
            </div>
            <p className='m-0 text-secondary'> {reviews} reviews / {rating} average </p>
          </div>
          <h5 className='w-75'>{title}</h5>
        </div>
      </div>
    </Link>
  )
}