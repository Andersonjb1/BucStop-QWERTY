# Game Submission Rejection Criteria (Acceptance Criteria)

This document outlines the conditions under which a game submission will be rejected, based on the validation logic found in the GamesController.cs file. Submissions are checked for field validity, file constraints, and content safety.

## Field Validation Failures (Game Metadata)

The submission will be rejected if any of the following conditions for the game's metadata are not met, as checked by the `ValidateGameSubmission` method:

| Field | Condition for Rejection | Error Message |
|-------|------------------------|---------------|
| Title | Is empty or contains only whitespace. | "Game title is required." |
| Title | Exceeds 50 characters in length. | "Game title must be 50 characters or less." |
| Title | Contains content identified as inappropriate by the filter. | "Game title contains inappropriate content." |
| Author | Is empty or contains only whitespace. | "Author name is required." |
| Author | Exceeds 100 characters in length. | "Author name must be 100 characters or less." |
| Description | Is empty or contains only whitespace. | "Game description is required." |
| Description | Is less than 20 characters in length. | "Description must be at least 20 characters." |
| Description | Exceeds 1000 characters in length. | "Description must be 1000 characters or less." |
| HowToPlay | Is empty or contains only whitespace. | "Instructions are required." |
| HowToPlay | Exceeds 1000 characters in length. | "Instructions must be 1000 characters or less." |
| ThumbnailUrl | Is provided, but is not a valid absolute HTTP or HTTPS URL. | "Thumbnail URL must be a valid HTTP/HTTPS URL." |

## JavaScript File Validation Failures

The submission will be rejected if the uploaded JavaScript file (`jsFile`) fails any of the following checks, as performed by `ValidateJavaScriptFile` and within `ValidateGameSubmissionForm`:

| Condition for Rejection | Error Message |
|------------------------|---------------|
| The uploaded file does not have a .js file extension. | "Only .js files are allowed for code uploads." |
| The file size is greater than 500KB (500,000 bytes). | "JavaScript file must be less than 500KB." / "JavaScript code is too large (max 500KB)." |
| The JavaScript code content contains potentially dangerous patterns. | "Code contains potentially dangerous patterns." |
| The system is unable to read the content of the JavaScript file. | "Unable to read JavaScript file content." |

## Dangerous Code Patterns

The "Code contains potentially dangerous patterns" rejection is triggered if the submitted JavaScript code (checked by `ContainsDangerousCode`) contains any of the following security-sensitive patterns:

- `eval(` (including variations like `eval()`)
- `Function(`
- `<script`
- `document.write`
- `innerHTML =`
- `outerHTML =`
- `.cookie`
- `localStorage`
- `sessionStorage`
- `XMLHttpRequest`
- `fetch(`
- `import(`

**NOTE:** Please take into consideration modifying these rules. We have had issues where these rules have been too strict in some cases, so the RegEx in the code can and should be modified. Note that just because the code is in the submission container does not mean that it is 100% safe.

## Server-Side Rejection (Post-Validation)

Although the primary validation is done upfront, the submission process itself also has points of failure that will lead to rejection/failure:

- If the application encounters an unexpected error while processing or saving the submission (`try-catch` block).
- If, during file saving, the file extension check fails (redundantly checking that only .js uploads are allowed).
- If an invalid path operation is detected during the secure file saving process, indicating an attempt to write outside the designated submissions directory.