import { React, useState } from 'react'
import { Link } from 'react-router-dom';
import { useEffect } from 'react';
import BigTagItem from '../components/homeComponents.js/BigTagItem';
import SmallTagItem from '../components/homeComponents.js/SmallTagItem';
import SearchRecipes from '../components/homeComponents.js/SearchRecipes';
import RecipeItem from '../components/homeComponents.js/RecipeItem';

export default function Home() {

  const bigTagItems = Array(4).fill(null).map((_, index) => (
    <BigTagItem key={index} id="2" name="DINNER" src="https://pinchofyum.com/wp-content/uploads/cropped-Tofu-and-Brown-Rice-Lettuce-Wraps-Square.png" />
  ));

  const smallTagItems = Array(9).fill(null).map((_, index) => (
    <SmallTagItem key={index} id="2" name="DINNER" src="https://pinchofyum.com/wp-content/uploads/cropped-Tofu-and-Brown-Rice-Lettuce-Wraps-Square.png" />
  ));

  const recipeItems = Array(3).fill(null).map((_, index) => (
    <RecipeItem key={index} id="3" date="January 8, 2024" title="Ridiculously Good Air Fryer Chicken Breast" description="My go-to everyday air fryer chicken! Thinly sliced chicken breast pieces, coated to the max in spices, plus a bit of brown sugar and cornstarch, and air fried to golden, juicy perfection." src="https://pinchofyum.com/wp-content/uploads/cropped-Tofu-and-Brown-Rice-Lettuce-Wraps-Square.png" />
  ));

  return (
    <section className='homeContainer'>
      <section style={{ height: "90px" }}>

      </section>
      <section className='heroContainer' style={{ background: "#f7f7f7" }}>
        <div className='m-auto pb-5' style={{ width: "60%" }}>
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
      <section className='recipeList'>
        <div className='m-auto pb-4' style={{ width: "60%" }}>
          <h3 className='mt-5 text-primary mb-5'>Our latest recipes</h3>
          {recipeItems}
          <div className='text-center'>
            <Link to="/recipes" className='btn btn-lg btn-primary'>VIEW MORE RECENT POSTS</Link>
          </div>
        </div>
      </section>
      <section className='followSection bg-primary p-5'>
        <div className='m-auto d-flex justify-content-center align-items-center' style={{ width: "60%", color: "white" }}>
          <h3 className="m-0 me-3">Follow us</h3>
          <Link to="/contact" className='rounded text-primary text-center me-3' style={{ background: "white", width: "40px", fontSize: "27px" }}>
            <i className="bi bi-instagram"></i>
          </Link>
          <Link to="/contact" className='rounded text-primary text-center me-3' style={{ background: "white", width: "40px", fontSize: "27px" }}>
            <i className="bi bi-pinterest"></i>
          </Link>
          <Link to="/contact" className='rounded text-primary text-center me-3' style={{ background: "white", width: "40px", fontSize: "27px" }}>
            <i className="bi bi-facebook"></i>
          </Link>
          <Link to="/contact" className='rounded text-primary text-center me-3' style={{ background: "white", width: "40px", fontSize: "27px" }}>
            <i className="bi bi-twitter"></i>
          </Link>
        </div>
      </section>
    </section>
  )
}