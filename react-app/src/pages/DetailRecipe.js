import React, { useState, useEffect } from 'react';
import { Link, useParams } from 'react-router-dom';
import ClassicEditor from '@ckeditor/ckeditor5-build-classic';
import RecipeItem from '../components/homeComponents.js/RecipeItem';
import SmallRecipeList from '../components/homeComponents.js/SmallRecipeList';

export default function DetailRecipe() {
  const [recipeData, setRecipeData] = useState([]);
  const [commentData, setCommentData] = useState([]);
  const [error, setError] = useState([]);

  const { id } = useParams();

  useEffect(() => {
    if (id) {
      fetch(`https://localhost:7147/api/Recipe/${id}`)
        .then(response => {
          if (!response.ok) {
            throw new Error(`Recipe not found! Status: ${response.status}`);
          }
          return response.json();
        })
        .then(data => {
          setRecipeData(data);

          let ingredients = document.getElementById("ingredients");
          let printBtn = document.createElement("a");

          let i = document.createElement("i");
          i.classList.add("bi");
          i.classList.add("bi-printer");

          printBtn.appendChild(document.createTextNode("Print Recipe "));
          printBtn.appendChild(i);
          printBtn.classList.add("btn");
          printBtn.classList.add("btn-light");
          printBtn.style.float = "right";

          printBtn.href = `../Print/${id}`;

          ingredients.appendChild(printBtn);
        })
        .catch(error => {
          setError(error.message);
        });

      fetch(`https://localhost:7147/api/Comment/GetCommentsByRecipe/${id}`)
        .then(response => {
          if (!response.ok) {
            throw new Error(`Comments not found! Status: ${response.status}`);
          }
          return response.json();
        })
        .then(data => {
          setCommentData(data);
          console.log(data);
        })
        .catch(error => {
          setError(error.message);
        });
    }

  }, [id]);

  return (id ?
    <>
      {recipeData.length != 0 ?
        <>
          <div className='m-auto w-60 ck-content'>
            <h1 className='py-5'>{recipeData.title}</h1>
            <a href="#ingredients" className="w-100 d-block bg-light text-center p-3 mb-5 text-decoration-none" style={{ fontSize: "20px" }}><i className="bi bi-arrow-down"></i> Jump to recipe</a>
            <div dangerouslySetInnerHTML={{ __html: recipeData.body }} />
          </div>

          <SmallRecipeList id={id} />
        </>
        :
        <p>Recipe does not exist</p>
      }
    </>
    :
    <>
      <p>No id given</p>
    </>
  );
}
