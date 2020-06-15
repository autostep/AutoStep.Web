@element
Feature: TextBox

    # Finding

    @finding
    Scenario Outline: Can locate text box

        Given I have navigated to /textbox in the basic application
        
        Then the <label> field should be displayed

    Examples:
        | title             | label         |
        | for attribute     | Label Fo      |
        | aria-label        | Aria-Labelled |
        | aria-labelledby   | Labelled By   |

    $expectingError: Expecting a single element, but found 0.
    @finding
    Scenario: Fails to Find TextBox with Wrong Label

        Given I have navigated to /textbox in the basic application
        
        Then the 'Not a Label' field should be displayed

    @entry
    Scenario Outline: Can type into textbox and assert contents

        Given I have entered '<value>' into the 'Label For' field
        
        Then the 'Label For' field should have the value '<value>'

    Examples:
        | value                                                                                                 |
        | Hello World                                                                                           |
        | ABCDEFGHIJKLMNOPQRSTUVXYZABCDEFGHIJKLMNOPQRSTUVXYZABCDEFGHIJKLMNOPQRSTUVXYZABCDEFGHIJKLMNOPQRSTUVXYZ  |

        
        

    