import { React, useState, useEffect } from 'react'

export default function CommentItem({ id, body, rating, authorId, date, userData }) {

  const [userName, setUserName] = useState([])
  const [error, setError] = useState([])

  const fullStars = Math.round(rating * 2) / 2;
  const hasHalfStar = fullStars % 1 !== 0;

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

  useEffect(() => {
    fetch(`https://localhost:7147/api/Account/name-from-id/${authorId}`, {
      method: 'GET',
      credentials: 'include'
    })
      .then(response => {
        if (!response.ok) {
          throw new Error(`Error! Status: ${response.status}`);
        }
        return response.text();
      })
      .then(data => {
        setUserName(data);
      })
      .catch(error => {
        setError(error.message);
        console.log(error);
      });
  }, []);

  return (
    <>
      <div className="comment-item mb-5 p-4 rounded bg-light" style={{ background: "white" }}>
        <div className="d-flex align-items-start">
          <img src="https://ckbox.cloud/nCX3ISMpdWvIZzPqyw4h/assets/_9A8c_VZOve3/images/377.png" className="border rounded-circle" style={{ width: "100px", aspectRatio: "1 / 1" }} />
          <div className="ms-4 ">
            <div className='d-flex'>
              <h5 className='me-3'>
                {userName}
              </h5>
              <div>
                {renderStars()}
              </div>
            </div>
            <p style={{ textAlign: "justify" }}>
              {body}
            </p>
          </div>
        </div>

        <div className="d-flex justify-content-between align-items-center mt-3">
          <p className="text-primary m-0">
            {date}
          </p>
          <a className="btn btn-danger">
            <i className="bi bi-trash"></i> &nbsp; Delete Comment
          </a>
        </div>
      </div>
    </>
  )
}