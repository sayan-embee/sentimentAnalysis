
---

## Sentiment Analysis App Documentation

### Overview
The Sentiment Analysis App analyzes audio data from .mp3 files using the Azure OpenAI Whisper model. The application processes user-uploaded audio files stored in Azure Blob Storage to extract the reason (motive of speakers), an audio summary, and an audio transcript. The analysis data is stored in a SQL Server database.

### Key Features
- **Audio Data Analysis**: Analyzes audio files to determine the speaker's motive, provides a summary, and generates a transcript.
- **Azure OpenAI Whisper Model**: Utilizes advanced AI for processing and analyzing audio content.
- **Data Storage**: Stores analysis results in Azure Blob Storage and SQL Server database.

### Workflow
1. **Audio File Upload and Analysis**
   - **Upload Audio File**: Users upload an .mp3 file by clicking the "Analyze Your Audio" button.
   - **POST API Call**: 
     ```
     POST https://YOUR-HOST-URL/api/openAI/audioSummary
     ```
     - **Controller**: `OpenAIController.cs`
     - **Process**:
       - Validates the file extension.
       - Uploads the file to Azure Blob Storage.
       - Upon successful upload, Azure OpenAI generates the reason, summary, and transcript using the response data.
       - Stores the analysis data in the SQL Server database.

2. **Fetch and Visualize Analysis Data**
   - **GET API Call**:
     ```
     GET https://YOUR-HOST-URL/api/openAI/getAudioSummary
     ```
     - **Controller**: `OpenAIController.cs`
     - **Function**: Retrieves all stored analysis data for visualization.

### Detailed API Endpoints

#### Upload and Analyze Audio File
- **Endpoint**: `https://YOUR-HOST-URL/api/openAI/audioSummary`
- **Controller**: `OpenAIController.cs`
- **Function**: Validates the .mp3 file, uploads it to Azure Blob Storage, and performs audio analysis using Azure OpenAI.
  - **Process**:
    1. Validates the file extension.
    2. Uploads the file to Azure Blob Storage.
    3. Uses Azure OpenAI to generate the reason, summary, and transcript.
    4. Stores the analysis data in the SQL Server database.

#### Fetch Audio Analysis Data
- **Endpoint**: `https://YOUR-HOST-URL/api/openAI/getAudioSummary`
- **Controller**: `OpenAIController.cs`
- **Function**: Retrieves all stored audio analysis data for display.

### How It Works
1. **Audio File Upload**:
   - User uploads a .mp3 file.
   - Application validates the file extension.
   - File is uploaded to Azure Blob Storage.
   - Azure OpenAI processes the uploaded file to generate:
     - **Reason**: The motive of the speakers.
     - **Summary**: A brief summary of the audio content.
     - **Transcript**: The full transcript of the audio.
   - Analysis data is stored in the SQL Server database.
   
2. **Data Retrieval and Visualization**:
   - User can fetch all stored audio analysis data via the GET API.
   - The application retrieves and visualizes the data for the user.

### Reference Interface
In the reference picture, users can upload a .mp3 file by clicking the "Analyze Your Audio" button. After analysis, users can view the stored data visualized below.

---

This documentation should provide a clear and comprehensive overview of the Sentiment Analysis App's functionality and workflow.
