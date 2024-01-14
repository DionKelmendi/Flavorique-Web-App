import { React, useState, useEffect } from 'react'
import { Link } from 'react-router-dom';
import RecipeListItem from './RecipeListItem';

export default function RecipeList({ }) {

  const recipeListItems = Array(12).fill(null).map((_, index) => (
    <RecipeListItem key={index} id="2" src="https://pinchofyum.com/wp-content/uploads/Chocolate-Chip-Cookies-183x183.jpg" title="The Best Soft Chocolate Chip Cookies" reviews="1658" rating="4.5" />
  ));

  return (
    <>
      <section>
        <div className='m-auto text-center w-60 py-5'>
          <h3 className='text-primary'><i className="bi bi-journal-text text-warning"></i> &nbsp; All Recipes &nbsp; <i className="bi bi-journal-text text-warning"></i></h3>
          <div className='d-flex flex-wrap'>
            {recipeListItems}
          </div>
          <p className='mt-5'><b>MORE RECIPES</b></p>
          <div className='paginationContainer'>
            <a className="btn btn-primary me-2"> 1 </a>
            <a className="btn btn-light me-2"> 2 </a>
            <a className="btn btn-light me-2"> ... </a>
            <a className="btn btn-light me-2"> 10 </a>
            <a className="btn border"> <b>&gt;</b> </a>
          </div>
        </div>
      </section>
    </>
  )
}