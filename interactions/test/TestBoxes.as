Feature: TextBox

    Scenario: Can Locate TextBox Using Label With For

        Given I have navigated to /textbox in the basic application
        
        Then the 'Label For' field should be displayed
        