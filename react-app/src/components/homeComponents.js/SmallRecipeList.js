import { React, useState, useEffect } from 'react'
import { Link } from 'react-router-dom';
import RecipeItem from './RecipeItem';

export default function SmallRecipeList({ }) {

  const recipeItems = Array(3).fill(null).map((_, index) => (
    <RecipeItem key={index} id="3" date="January 8, 2024" title="Ridiculously Good Air Fryer Chicken Breast" description="My go-to everyday air fryer chicken! Thinly sliced chicken breast pieces, coated to the max in spices, plus a bit of brown sugar and cornstarch, and air fried to golden, juicy perfection." src="https://pinchofyum.com/wp-content/uploads/cropped-Tofu-and-Brown-Rice-Lettuce-Wraps-Square.png" />
  ));

  return (
    <section className='recipeList'>
      <div className='m-auto pb-4 w-60'>
        <h3 className='mt-5 text-primary mb-5'>Our latest recipes</h3>
        {recipeItems}
        <div className='text-center'>
          <Link to="/Recipes/All?sort=recent" className='btn btn-lg btn-primary'>VIEW MORE RECENT POSTS</Link>
        </div>
      </div>
    </section>
  )
}