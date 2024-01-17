import React, { useState, useEffect, useRef } from 'react';
import { CKEditor } from '@ckeditor/ckeditor5-react';
import ClassicEditor from '@ckeditor/ckeditor5-build-classic';

export default function CreateRecipe() {
  const [bodyData, setBodyData] = useState('');
  const [x, setX] = useState(0);
  const rotatorRef = useRef(null);

  useEffect(() => {
    rotatorRef.current = document.querySelector(".rotator");

    ClassicEditor
      .create(document.querySelector('#editor'), {
        ckbox: {
          tokenUrl: "https://localhost:7147/api/CKBox/token",
          theme: 'lark'
        },
        removePlugins: ['FormatPainter', 'Table'],
      })
      .then(editor => {
        editor.model.document.on('change:data', () => {
          handleEditorChange(null, editor);
        });
      })
      .catch(error => {
        console.log(error);
      });
  }, []);

  const move = (direction) => {
    const updatedX = direction === 'left' ? x - 1 : x + 1;
    rotatorRef.current.style.transform = `translateX(${-33.3 * updatedX}%)`;
    setX(updatedX);
  };

  const handleEditorChange = (event, editor) => {
    const data = editor.getData();
    setBodyData(data);
  };

  const handleFormSubmit = (e) => {
    e.preventDefault();

    const currentDate = new Date();

    const jsonData = {
      "id": 0,
      "title": "hi",
      "body": bodyData,
      "createdDateTime": currentDate.toISOString(),
      "authorId": null,
      "comments": null,
      "author": null
    };

    fetch("https://localhost:7147/api/Recipe", {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      credentials: 'include',
      body: JSON.stringify(jsonData),
    })
      .then(response => {
        if (!response.ok) {
          throw new Error(`HTTP error! Status: ${response.status}`);
        }
        return response.json();
      })
      .then(data => {
        console.log('Success:', data);
      })
      .catch(error => {
        console.error('Error:', error);
      });

    console.log(bodyData);
  };

  return (
    <section className='createRecipeContainer'>
      <section>
        <div className='w-60 m-auto pt-5 overflow-hidden'>
          <div className='rotator d-flex overflow-hidden' style={{ width: "300%", transition: "0.3s all ease-in" }}>
            <div className='rotatingItem border' style={{ width: "33.3%" }}></div>
            <div className='rotatingItem' style={{ width: "33.3%" }}>
              <div id="editor" />
            </div>
            <div className='rotatingItem border' style={{ width: "33.3%" }}></div>
          </div>
          <button onClick={() => move('left')} className={`btn btn-primary mt-3 ${x === 0 ? 'd-none' : ''}`} style={{ float: "left" }}>
            <i className="bi bi-arrow-left"></i> Back
          </button>
          <button onClick={() => move('right')} className={`btn btn-primary mt-3 ${x === 2 ? 'd-none' : ''}`} style={{ float: "right" }}>
            Next <i className="bi bi-arrow-right"></i>
          </button>
          <form onSubmit={handleFormSubmit}>
            <button type='submit' className={`btn btn-primary mt-3 ${x !== 2 ? 'd-none' : ''}`} style={{ float: "right" }}>
              Submit Recipe <i className="bi bi-plus-square"></i>
            </button>
          </form>
        </div>
      </section>
    </section>
  );
}
