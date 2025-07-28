# SubCategory Image Upload Functionality

## Overview
Image upload functionality has been successfully added to the SubCategory entity. This allows you to upload and manage images for subcategories, similar to how it works for categories.

## Changes Made

### 1. Entity Updates
- **SubCategory Entity**: Added `Image` property to store image path
- **Database Migration**: Created and applied migration `AddImageToSubCategory`

### 2. DTO Updates
- **SubCategoryDTO**: Added `Image` property to include image information in responses
- **SubCategoryMapper**: Updated to map the Image property

### 3. Service Layer Updates
- **ISubCategoryService**: Added image upload and delete method signatures
- **SubCategoryService**: Implemented image upload and delete functionality with IFileService integration

### 4. Controller Updates
- **SubCategoryController**: Added image upload and delete endpoints
- **FormDataOperationFilter**: Already configured for SubCategory image parameters

### 5. File System
- Created `wwwroot/assets/subcategories/` directory for storing uploaded images

## API Endpoints

### Upload SubCategory Image
```
POST /api/SubCategory/{subCategoryId}/upload-image
Content-Type: multipart/form-data

Parameters:
- subCategoryId (string): The ID of the subcategory
- imageFile (IFormFile): The image file to upload

Response:
{
  "success": true,
  "message": "SubCategory image uploaded successfully",
  "data": {
    "success": true,
    "message": "Image uploaded successfully",
    "imagePath": "subcategories/filename.jpg",
    "imageUrl": "https://your-domain.com/assets/subcategories/filename.jpg"
  }
}
```

### Delete SubCategory Image
```
DELETE /api/SubCategory/{subCategoryId}/delete-image

Parameters:
- subCategoryId (string): The ID of the subcategory

Response:
{
  "success": true,
  "message": "SubCategory image deleted successfully",
  "data": true
}
```

## Usage Examples

### Using cURL to upload an image:
```bash
curl -X POST "https://your-api.com/api/SubCategory/123e4567-e89b-12d3-a456-426614174000/upload-image" \
  -H "Content-Type: multipart/form-data" \
  -F "imageFile=@/path/to/your/image.jpg"
```

### Using cURL to delete an image:
```bash
curl -X DELETE "https://your-api.com/api/SubCategory/123e4567-e89b-12d3-a456-426614174000/delete-image"
```

## Features

1. **Image Validation**: Validates file type and size (supports jpg, jpeg, png, gif, bmp, webp up to 5MB)
2. **Automatic File Management**: Files are stored in organized directory structure
3. **Database Integration**: Image paths are stored in the database and linked to subcategories
4. **Error Handling**: Comprehensive error handling with meaningful messages
5. **Logging**: All operations are logged for debugging purposes

## File Storage
- Images are stored in: `wwwroot/assets/subcategories/`
- File naming includes timestamp to prevent conflicts
- Original file extension is preserved

## Security Considerations
- File type validation prevents malicious uploads
- File size limits prevent abuse
- Endpoints can be secured with authorization attributes (currently commented out)

## Testing
To test the functionality:
1. Create a subcategory first using the existing POST endpoint
2. Use the new upload endpoint to add an image
3. Verify the image appears in the subcategory details
4. Test the delete endpoint to remove the image

The implementation follows the same pattern as the Category image upload functionality, ensuring consistency across the application. 