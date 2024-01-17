import React, { useState, useEffect } from 'react';
import { Link, useParams } from 'react-router-dom';
import ClassicEditor from '@ckeditor/ckeditor5-build-classic';
import SmallRecipeItem from '../components/recipeComponents.js/SmallRecipeItem';

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

            <section className="d-flex flex-column align-items-center justify-content-evenly text-center mt-5">
              <h1 className="align-self-start mt-5">Similar Recipes</h1>
              <div className="w-100 d-flex flex-column align-items-center justify-content-center">
                <div className="border-bottom rounded d-flex align-items-center p-4" style={{ height: "250px" }}>
                  <img className=" h-100" style={{ aspectRatio: "1 / 1" }} />
                  <div className="p-5" style={{ textAlign: "left" }}>
                    <p className="text-secondary">November 14, 2023</p>
                    <h3 className="fw-bolder">Title</h3>
                    <p style={{ textAlign: "justify" }}>This shredded beef Texas chili is rich and hearty, made with 8 simple ingredients! Just saucy chunks of tender beef that shreds apart with the lightest pull of a fork. YUM.</p>
                    <a asp-controller="Recipe" asp-action="Details" asp-route-id="1" style={{ fontSize: "18px", letterSpacing: "1px" }}>Continue Reading <i className="bi bi-arrow-right"></i></a>
                  </div>
                </div>
              </div>
            </section>
            <SmallRecipeItem id="2" src="https://pinchofyum.com/wp-content/uploads/Chocolate-Chip-Cookies-183x183.jpg" title="The Best Soft Chocolate Chip Cookies" reviews="1658" rating="4.9" />
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
