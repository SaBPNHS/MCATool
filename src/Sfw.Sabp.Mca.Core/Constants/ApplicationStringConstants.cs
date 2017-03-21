namespace Sfw.Sabp.Mca.Core.Constants
{
    public static class ApplicationStringConstants
    {
        public const string DecisionConfirmationText = "Is the decision to be made specific and clearly defined?";

        public const string DecisionPromptText =
            "The decision must be about a specific treatment or choice and not a vague question e.g. where would you like to live?";

        public const string Stage1ShortDescription = "section1";
        public const string Stage1Text = "Section 1 - Defining the decision to be made.";
        public const string AssessmentStartDateText = "Assessment Start Date:";
        public const string AssessmentCompleteDateText = "Assessment Completed Date:";
        public const string ClinicianNameText = "Assessor's Name:";
        public const string McaSubject = "MENTAL CAPACITY ACT - CAPACITY ASSESSMENT REPORT";
        public const string DecisionToBeMade = "DECISION TO BE MADE:";
        public const string Name = "Name:";
        public const string Dob = "D.O.B.:";
        public const string NhsNumber = "NHS NUMBER:";
        public const string AssessmentTerminationReasonString = "Assessment Termination Reason: ";
        public const string RoleText = "Role:";
        public const string DecisionMakerText = "Decision Maker:";
        public const string DateOfBirthValueKey = "DateOfBirth";
        public const string DateofBirthViewModelDayKey = "DateOfBirthViewModel.Day";
        public const string DateofBirthViewModelMonthKey = "DateOfBirthViewModel.Month";
        public const string DateofBirthViewModelYearKey = "DateOfBirthViewModel.Year";
        public const string EditPersonAuditValues = "ClinicalSystemId,NhsNumber,FirstName,LastName,GenderId," + DateOfBirthValueKey;

        // validation strings
        public const string RoleIdMandatory = "Role is mandatory";
    }
}
