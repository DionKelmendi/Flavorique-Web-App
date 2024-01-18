import { React, useState, useEffect, useRef } from 'react'
import CommentItem from './CommentItem';

export default function CommentList({ recipeId }) {

  const [commentData, setCommentData] = useState([]);
  const [error, setError] = useState([]);
  const containerRef = useRef(null);
  const toggleRef = useRef(null);

  useEffect(() => {
    containerRef.current = document.querySelector(".comment-container");
    toggleRef.current = document.querySelector(".commentToggler");

    console.log(containerRef.current);
    console.log(toggleRef.current);

    fetch(`https://localhost:7147/api/Comment/GetCommentsByRecipe/${recipeId}`, {
      method: 'GET',
      credentials: 'include'
    })
      .then(response => {
        if (!response.ok) {
          throw new Error(`Error! Status: ${response.status}`);
        }
        return response.json();
      })
      .then(data => {
        setCommentData(data);
        console.log(data);
      })
      .catch(error => {
        setError(error.message);
        console.log(error);
      });
  }, []);

  const comments = commentData.map(comment => (
    <CommentItem
      key={comment.id}
      id={comment.id}
      body={comment.body}
      rating={comment.rating}
      authorId={comment.authorId}
      date={
        new Date(comment.createdDateTime).toLocaleDateString('en-US', {
          year: 'numeric',
          month: 'long',
          day: 'numeric',
        })
      }
    />
  ));

  function toggleComments() {
    if (containerRef.current.style.height == "0px") {
      containerRef.current.style.height = "auto";
      toggleRef.current.innerHTML = "Hide comments";
    } else {
      containerRef.current.style.height = "0px";
      toggleRef.current.innerHTML = "Show comments";
    }
  }

  return (
    <section className='mt-5 py-5' style={{ background: "#f7f7f7" }}>

      <div className='m-auto w-60'>
        <div className="d-flex justify-content-between align-items-center">
          <h4 className="text-primary">{comments.length} Comments</h4>
          <a className="commentToggler text-decoration-none text-secondary-emphasis" onClick={toggleComments} style={{ cursor: "pointer" }}>Close comments</a>
        </div>

        <hr className="mb-5" />

        <div className="comment-container overflow-hidden">
          {comments}
        </div>
      </div>
    </section>
  )
}