## Project Description
This project is a .NET Core API that allows users to download multiple images asynchronously and at a time they can download maximum "MaxDownloadAtOnce" number of images. The API stores the downloaded images in the server's localhost and gives them unique names. Additionally, the API allows users to retrieve a specific image by its name as a base64String.

## API Endpoints
### POST /download
This endpoint accepts a model that contains a list of image URLs and a maximum number of images that can be downloaded at once. The API downloads the images asynchronously and returns a model that contains the status of the operation, a message, and a dictionary of downloaded image URLs and their unique names.
### GET /get-image-by-name/{image_name}
This endpoint accepts the name of an image that was previously downloaded using the /download endpoint. The API retrieves the image by its name and returns it as a base64String.

## Getting Started
To get started with this project, you need to clone the repository to your local machine and open it in your preferred IDE. Make sure you have .NET Core 3.1 installed.
