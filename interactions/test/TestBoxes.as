@element
Feature: TextBox
    Verifies default input field behaviour in AutoStep.    

    # Finding

    @finding
    Scenario Outline: Can locate text box

        Given I have navigated to /textbox in the basic application
        
        Then the <label> field should be displayed

    Examples:
        | title             | label         |
        | for attribute     | Label For     |
        | aria-label        | Aria-Labelled |
        | aria-labelledby   | Labelled By   |
        | disabled          | Disabled      |

    $expectingError: Expecting a single element, but found 0.
    @finding
    Scenario: Fails to Find TextBox with Wrong Label

        Given I have navigated to /textbox in the basic application
        
        Then the 'Not a Label' field should be displayed

    $expectingError: Expecting element at index 0 to be enabled, but the element was disabled.
    @can-disable
    Scenario: Cannot Type into Disabled Field

        Given I have navigated to /textbox in the basic application
        
        Given I have clicked the Disabled field
        
        Given I have entered 'ABC' into the 'Disabled' field

    @can-disable
    Scenario Outline: Can check enabled field state

        Given I have navigated to /textbox in the basic application
        
        Then the <label> field should be enabled
    
    Examples:
        | title             | label         |
        | for attribute     | Label For     |
        | aria-label        | Aria-Labelled |
        | aria-labelledby   | Labelled By   |

    $expectingError: Expecting element at index 0 to be enabled, but the element was disabled.
    @can-disable
    Scenario: Cannot enter into disabled field

        Given I have navigated to /textbox in the basic application
        
        Given I have entered '1234' into the 'Disabled' field
        
    @entry
    Scenario Outline: Can type into textbox and assert contents

        Given I have entered '<value>' into the 'Label For' field
        
        Then the 'Label For' field should have the value '<value>'

    Examples:
        | value                                                                                                 |
        | Hello World                                                                                           |
        | ABCDEFGHIJKLMNOPQRSTUVXYZABCDEFGHIJKLMNOPQRSTUVXYZABCDEFGHIJKLMNOPQRSTUVXYZABCDEFGHIJKLMNOPQRSTUVXYZ  |

        
        

    