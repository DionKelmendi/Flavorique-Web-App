import React, { useState, useEffect } from 'react';
import { Link, useParams } from 'react-router-dom';

export default function PrintRecipe({ }) {
  const [recipeData, setRecipeData] = useState([]);
  const [error, setError] = useState([]);

  const { id } = useParams();

  useEffect(() => {
    if (id) {
      const apiUrl = `https://localhost:7147/api/Recipe/print?id=${id}`;

      fetch(apiUrl)
        .then(response => {
          if (!response.ok) {
            throw new Error(`Recipe not found! Status: ${response.status}`);
          }
          return response.json();
        })
        .then(data => {
          setRecipeData(data);
          console.log(recipeData);
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
          <div className='w-60 m-auto mt-5 mb-2'>
            <a href={`https://localhost:7147/api/Recipe/printPDF?id=${id}`}>
              <button>Get Recipe as PDF</button>
            </a>
          </div>
          <div className='w-60 p-5 mx-auto' style={{ border: "5px solid gray" }}>

            <div className='d-flex justify-content-between'>
              <h1 className='py-5'>{recipeData.title}</h1>
              <img height={150} style={{ aspectRatio: "1 / 1" }} src={recipeData.image} />
            </div>
            <div dangerouslySetInnerHTML={{ __html: recipeData.body }} />
          </div>
          <div className='w-60 m-auto mt-2 mb-5'>
            <Link to={"/Recipes/Detail/" + id}><button>Go back</button></Link>
          </div>
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