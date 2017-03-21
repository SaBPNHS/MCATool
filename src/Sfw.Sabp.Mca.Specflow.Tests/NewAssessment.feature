Feature: NewAssessment
	
@mytag
Scenario: On successful creation of an assessment the user should be redirected to Question Page
Given The user has entered all the information
When He Clicks on Create button
Then He should be redirected to the Question Page

Scenario: Create should fail if stage1decisiontobemade is missing
Given the user has not entered the decision to be made
When click on Create button
Then he should be shown the error message "Decision to be made is mandatory"

Scenario: Create should fail if stage1decisionclearlymade is false
Given the user has not selected the stage 1 decision clearly made checkbox
When click on Create button
Then he should be shown the error message "Please confirm that the decision is clearly defined"

Scenario: Create should fail if assessment start date is future date
Given the user has entered future date for assessment start date
When clicked on Create button
Then he should be shown the error message "Date assessment started must not be in the future"