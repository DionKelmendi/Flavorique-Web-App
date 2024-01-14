import { React, useState, useEffect } from 'react'
import { Link } from 'react-router-dom';

export default function SmallRecipeItem({ id, src, title, reviews, rating }) {

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
    <Link to={"/recipe/" + id} className='text-decoration-none' style={{ width: "33%", color: "black" }}>
      <div className='d-flex m-3'>
        <img height={100} width={100} src={src} className='me-3' />
        <div className='d-flex flex-column justify-content-between'>
          <h5>{title}</h5>
          <div>
            <div className='starContainer text-warning'>
              {renderStars()}
            </div>
            <p className='m-0 text-secondary'> {reviews} reviews / {rating} average </p>
          </div>
        </div>
      </div>
    </Link>
  )
}