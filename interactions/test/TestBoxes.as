Feature: TextBox

    Scenario: Can Locate TextBox Using Label With For

        Given I have navigated to /textbox in the basic application
        
        Then the 'Label For' field should be displayed
    
    $expectingError
    Scenario: Fails to Find TextBox with Wrong Label

        Given I have navigated to /textbox in the basic application
        
        Then the 'Not a Label' field should be displayed