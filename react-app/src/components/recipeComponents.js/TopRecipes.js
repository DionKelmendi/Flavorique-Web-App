import { React, useState, useEffect } from 'react'
import { Link } from 'react-router-dom';
import SmallRecipeItem from './SmallRecipeItem';

export default function TopRecipes({ }) {

  const smallRecipeItems = Array(9).fill(null).map((_, index) => (
    <SmallRecipeItem id="2" src="https://pinchofyum.com/wp-content/uploads/Chocolate-Chip-Cookies-183x183.jpg" title="The Best Soft Chocolate Chip Cookies" reviews="1658" rating="4.9" />
  ));

  return (
    <>
      <section className='topRecipes'>
        <div className='m-auto text-center w-60 pt-5'>
          <h3 className='text-primary'><i className="bi bi-trophy text-warning"></i> &nbsp; Top rated recipes &nbsp; <i className="bi bi-trophy text-warning"></i></h3>
          <p className='w-50 m-auto'>Out of all the many recipes on Flavorique, these are our shining stars - the recipes we come back to again and again (and again).</p>

          <div className='recipeContainer d-flex flex-wrap justify-content-between text-start mt-5'>
            {smallRecipeItems}
          </div>
          <div className="d-grid w-60 m-auto p-5">
            <Link to="/Recipes/All" className='btn btn-lg btn-primary'>
              <b>VIEW ALL RECIPES</b>
            </Link>
          </div>
        </div>
      </section>
    </>
  )
}
