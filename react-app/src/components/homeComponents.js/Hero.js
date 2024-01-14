import { React, useState, useEffect } from 'react'
import BigTagItem from './BigTagItem'
import SmallTagItem from './SmallTagItem'
import SearchRecipes from './SearchRecipes'
import { Link } from 'react-router-dom';

export default function Hero({ }) {

  const bigTagItems = Array(4).fill(null).map((_, index) => (
    <BigTagItem key={index} id="2" name="DINNER" src="https://pinchofyum.com/wp-content/uploads/cropped-Tofu-and-Brown-Rice-Lettuce-Wraps-Square.png" />
  ));

  const smallTagItems = Array(9).fill(null).map((_, index) => (
    <SmallTagItem key={index} id="2" name="DINNER" src="https://pinchofyum.com/wp-content/uploads/cropped-Tofu-and-Brown-Rice-Lettuce-Wraps-Square.png" />
  ));

  return (
    <>
      <section className='heroContainer' style={{ background: "#f7f7f7" }}>
        <div className='m-auto pb-5 w-60'>
          <div className='d-flex w-100 justify-content-center p-5' style={{ gap: "15px" }}>
            {bigTagItems}
          </div>
          <div className='d-flex w-100 justify-content-center' style={{ gap: "30px" }}>
            {smallTagItems}
          </div>
          <div className='d-flex w-100 justify-content-center align-items-center mt-3' style={{ gap: "20px" }}>
            <SearchRecipes />
          </div>
        </div>
      </section>
    </>
  )
}