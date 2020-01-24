﻿namespace SIM.Tool.Windows
{
  public static class WizardPipelinesConfig
  {
    public const string Contents = @"<configuration>
  <pipelines>
    <download8 title=""Downloading Sitecore"">
      <processor type=""SIM.Tool.Windows.Pipelines.Download8.Download8Processor, SIM.Tool.Windows""
                  title=""Downloading packages"" />
    </download8>
  </pipelines>
  <wizardPipelines>
    <agreement title=""SIM License Agreement"" startButton=""Accept"" finishText=""Thank you"">
      <steps afterLastStep=""SIM.Tool.Windows.SaveAgreement, SIM.Tool.Windows"">
        <step name=""Welcome message"" type=""SIM.Tool.Windows.UserControls.ConfirmStepUserControl, SIM.Tool.Windows""
              param=""PLEASE READ IT CAREFULLY! You can see this wizard because it is the first time Sitecore Instance Manager was executed in this user account after installation or update. You should accept license agreement to use it. It was taken from http://marketplace.sitecore.net and most likely you already accepted it before downloading, but just in case please do it again here."" />
        <step name=""License agreement from marketplace.sitecore.net""
              type=""SIM.Tool.Windows.UserControls.ConfirmStepUserControl, SIM.Tool.Windows""
              param=""YOU SHOULD CAREFULLY READ THE FOLLOWING TERMS AND CONDITIONS BEFORE USING THIS SOFTWARE (THE “SITECORE SOFTWARE”). ANY USE OF THE SITECORE SOFTWARE IS SUBJECT TO YOUR FULL ACCEPTANCE OF THIS LICENSE AGREEMENT.

Sitecore License Agreement

LICENSEE’S USE OF THE SITECORE SOFTWARE IS SUBJECT TO LICENSEE’S FULL ACCEPTANCE OF THE TERMS, CONDITIONS, DISCLAIMERS AND LICENSE RESTRICTIONS SET FORTH IN THIS AGREEMENT.

1. License Grant : Upon payment in full of the license fee, Licensor grants Licensee a non-exclusive, perpetual, non-transferable, non-assignable, non-sublicensable license, without time limitations, to use the Sitecore Software in supported configurations as described in the Documentation, in compliance with all applicable laws, in object code form only, exclusively for management of Licensee’s own current and future web infrastructure , subject to the terms and conditions set forth in this Agreement. Except as expressly authorized by this Agreement, “Licensee” as used herein does not include any other entity or person, including any present or future subsidiary or affiliate of Licensee, or any entity or person owning any interest in Licensee at present or in the future. “Sitecore Software” means the software that is licensed by Licensor in this Agreement, and any future Upgrades and Patches, as those terms are defined in Section 6 of this Agreement, that the Licensee may receive in accordance with the terms of the Agreement. “Documentation” means the resources made available in the reference section of the Sitecore Developer Network setting forth the then-current functional, operational, and performance capabilities of the Sitecore Software (http://sdn.sitecore.net/documentation).
 
1.1 License Key : Licensee will be provided a License Key that gives Licensee access to the Sitecore Software. The Sitecore Software may be used only on the equipment with the features and limitations specified in the License Key. The License Key shall be time-limited until full payment of the license fee has been received by Licensor. The License Key may limit the number of installations of the Sitecore Software and the capacity of the Servers on which the Sitecore Software may be installed. For purposes of this Agreement, one Server is defined as a physical or virtual Server with a processing power for the web process equivalent to at most eight CPU cores. For example, one physical Server may contain eight single-core CPUs, four dual-core CPUs, two quad-core CPUs or one eight-core CPU. A Server with three or four quad-core CPUs would therefore count as two Servers. A virtual server’s processing power will be counted as the number of physical cores allocated or processor core equivalents allocated to the virtual server.  For example, one virtual server with two physical processor cores allocated, or with the equivalent of two simulated processor cores allocated, would be counted as the equivalent of two CPU cores and therefore one Server.

1.2 Intellectual Property Rights : The Sitecore Software includes the following patents: U.S. Patent Nos. 7,856,345 and 8,255,526. Ownership of the Sitecore Software, and all worldwide rights, title and interest in and to the Intellectual Property associated with the Sitecore Software shall remain solely and exclusively with Licensor or with third parties that license modules included with the Sitecore Software. Licensee shall retain intact all applicable Licensor copyright, patent and/or trademark notices on and in all copies of the Sitecore Software. All rights, title, and interest in Sitecore Software not expressly granted to Licensee in this Agreement are reserved by Licensor. “Intellectual Property” as used in this Agreement means any and all patents, copyrights, trademarks, service marks and trade names (registered and unregistered), trade secrets, know-how, inventions, licenses and all other proprietary rights throughout the world related to the authorship, origin, design, utility, process, manufacture, programming, functionality and operation of Sitecore Software and its Derivative Works.

1.3 Confidential Information :   The term “Confidential Information” shall include any information, whether tangible or intangible, including, but not limited to, techniques, discoveries, inventions, ideas, processes, software (in source or object code form), designs, technology, technical specifications, flow charts, procedures, formulas, concepts, any financial data, and all business and marketing plans and information, in each case which is maintained in confidence by the disclosing party (“Disclosing Party”) and disclosed to the other party (“Recipient”) hereunder. The failure by the Disclosing Party to designate any tangible or intangible information as Confidential Information shall not give Recipient the right to treat such information as free from the restrictions imposed by this Agreement if the circumstances would lead a reasonable person to believe that such information is Confidential Information. Confidential Information does not include information which Recipient documents (a) is now, or hereafter becomes, through no act or failure to act on the part of Recipient, generally known or available to the public; (b) was rightfully in Recipient’s possession prior to disclosure by the Disclosing Party; (c) becomes rightfully known to Recipient, without restriction, from a source other than the Disclosing Party and without any breach of duty to the Disclosing Party;  (d) is developed independently by Recipient without use of or reference to any of the Confidential Information and without violation of any confidentiality restriction contained herein; or (e) is approved by the Disclosing Party for disclosure without restriction, in a written document executed by a duly authorized officer of the Disclosing Party. Recipient shall hold the Confidential Information received from the Disclosing Party in strict confidence and shall not, directly or indirectly, disclose it, except as expressly permitted herein. Recipient shall promptly notify the Disclosing Party upon learning of any misappropriation or misuse of Confidential Information disclosed hereunder. Notwithstanding the foregoing, Recipient shall be permitted to disclose Confidential Information pursuant to a judicial or governmental order, provided that Recipient provides the Disclosing Party reasonable prior notice, and assistance, to contest such order.
 
1.4 Restrictions on Use : Except as expressly authorized by applicable law or by Licensor in writing, Licensee shall not copy, in whole or in part, the Sitecore Software or Documentation, or modify, disassemble, decompress, reverse compile, reverse assemble, reverse engineer, or translate any portion of the Sitecore Software. Licensee shall not rent, lease, lend, distribute, sell, assign, license, or otherwise transfer the Sitecore Software, or create Derivative Works of the Sitecore Software. Additionally, Licensee may use embedded third-party components in the Sitecore Software exclusively as described in the OEM section of the Sitecore Developer Network ( http://sdn.sitecore.net/oem ).

1.5  Derivative Works : “Derivative Works” as used herein means any software program (whether in source code or object code), and all copies thereof, developed by or on behalf of Licensee based on or derived from any part of the Sitecore Software, including without limitation any revision, modification, enhancement, translation (including compilation or recapitulation by computer), abridgment, condensation, expansion, or any other form in which the Sitecore Software may be recast, transformed or adapted, and that, if prepared without Licensor's authorization, would constitute a patent, copyright or trade secret infringement of the Sitecore Software, or would otherwise constitute an unauthorized use of Licensor’s Confidential Information. In the event any such Derivative Works are created, Licensor shall own all right, title, and interest in and to such Derivative Works. If, under the operation of local law or otherwise, Licensee or such third party comes to have any rights associated with such Derivative Works, Licensee hereby and shall automatically assign all such rights to Licensor for no additional consideration. For avoidance of doubt, Licensor shall claim no intellectual property interest or legal interest of any kind in any code created by Licensee to facilitate its authorized use of the Sitecore Software so long as the creation of such code does not constitute a Derivative Work or violate any other provision of this Agreement.

1.6 Third Party Use and Rights : Licensee may authorize third parties to assist Licensee in the management, editing, and hosting of web-based content created by use of the Sitecore Software, provided that: (1) such activities are within the scope of the activities Licensee is itself authorized to perform under this Agreement; (2) such third party’s acts are primarily for the direct or indirect benefit of Licensee; and (3) such third parties are not charged a fee by Licensee for such activities. Licensee is prohibited from using the Sitecore Software as an Application Software Provider or in any time-sharing or other commercial arrangement of any kind that makes the Sitecore Software available to third parties primarily for the third party’s own uses. Except as expressly stated in this Agreement, no third party has any rights under this Agreement. Licensee is fully liable to the extent allowed by law for any unauthorized use of the Sitecore Software by third parties caused by any acts or omissions of Licensee.

1.7 License Verification : The Sitecore Software periodically transmits the following information to a server maintained by Licensor: License Key ID, licensee name, hostname (Licensee’s website URL), host IP, Sitecore Software version, and directory installation path. This information is used by Licensor to send periodic communications to Licensee relevant to use of the Sitecore Software, and to verify, at Licensor’s election, that the Sitecore Software is installed in accordance with the terms of this Agreement.

 

2 . Limited Warranties :
 
2.1 Licensor’s Limited Warranty : Licensor expressly warrants that the Sitecore Software provided to Licensee will work for a period of ninety (90) days following the effective date of this Agreement according to the Documentation. In the event any such Sitecore Software does not operate according to the Documentation during this Limited Warranty Period, Licensor shall repair or replace the Sitecore Software and such repair or replacement shall be Licensee’s sole and exclusive remedy. 

2.2 Virus/Malicious Code Warranty : Licensor and Licensee warrant that they will use commercially reasonable virus and malicious code detection software programs to test any electronic files, including electronic communications, prior to any delivery or upon receiving such communications and that the parties will continue to take such steps with respect to exchanging electronic files and communications pursuant to this Agreement. In the event either party detects any computer virus or malicious code it shall immediately notify the other party and where possible shall promptly provide revised replacement files in the event any such computer virus or malicious code is detected.

3. DISCLAIMER OF WARRANTIES: EXCEPT AS EXPRESSLY SET FORTH IN SECTION 2 ABOVE, THE SITECORE SOFTWARE AND THE DOCUMENTATION ARE SUPPLIED TO LICENSEE 'AS IS.' LICENSOR (DEFINED IN THIS SECTION AS LICENSOR’S PARENT, AFFILIATES, SUBSIDIARIES, DISTRIBUTORS AND THEIR RESPECTIVE OFFICERS, DIRECTORS AND EMPLOYEES) MAKES NO WARRANTY, EXPRESS OR IMPLIED, AS TO THE SITECORE SOFTWARE, THE DOCUMENTATION, THE OPERATION OF THE SITECORE SOFTWARE, OR ANY OTHER GOODS OR SERVICES RENDERED BY LICENSOR TO LICENSEE, INCLUDING BUT NOT LIMITED TO, ANY IMPLIED WARRANTIES OF MERCHANTABILITY, PERFORMANCE, ACCURACY, NON-INFRINGEMENT, FITNESS FOR A PARTICULAR PURPOSE AND ALL WARRANTIES ARISING FROM COURSE OF DEALING OR USAGE OF TRADE, EXCEPT TO THE EXTENT ANY WARRANTIES IMPLIED BY LAW CANNOT BE WAIVED. LICENSOR EXPRESSLY DISCLAIMS ANY AND ALL EXPRESS, IMPLIED OR STATUTORY WARRANTIES THAT THE SITECORE SOFTWARE WILL RUN PROPERLY ON ANY HARDWARE, THAT THE SITECORE SOFTWARE WILL MEET LICENSEE’S REQUIREMENTS OR OPERATE IN THE COMBINATIONS WHICH MAY BE SELECTED FOR USE BY LICENSEE, OR THAT THE OPERATION OF THE SITECORE SOFTWARE WILL BE UNINTERRUPTED OR ERROR FREE, OR THAT ALL ERRORS WILL BE CORRECTED. LICENSOR MAKES NO EXPRESS OR IMPLIED WARRANTY OF ANY KIND REGARDING ANY SEPARATELY LICENSED SOFTWARE THAT MAY BE USED WITH THE SITECORE SOFTWARE.
 
4. LIMITATION OF LIABILITY: LICENSOR (DEFINED IN THIS SECTION AS LICENSOR’S PARENT, AFFILIATES, SUBSIDIARIES, DISTRIBUTORS AND THEIR RESPECTIVE OFFICERS, DIRECTORS AND EMPLOYEES) SHALL NOT BE LIABLE TO LICENSEE OR ANY THIRD PARTY FOR ANY INDIRECT, INCIDENTAL, SPECIAL, CONSEQUENTIAL, PUNITIVE, OR EXEMPLARY DAMAGES ARISING OUT OF OR RELATED TO THIS AGREEMENT UNDER ANY LEGAL THEORY, INCLUDING BUT NOT LIMITED TO, LOST PROFITS, LOST DATA OR BUSINESS INTERRUPTION, THE COST OF RECOVERING ANY DATA, INFRINGEMENT, OR THE COST OF SUBSTITUTE SOFTWARE, EVEN IF LICENSOR HAS BEEN ADVISED OF, KNOWS OF, OR SHOULD HAVE KNOWN, OF THE POSSIBILITY OF SUCH DAMAGES. LICENSOR’S AGGREGATE LIABILITY UNDER THIS AGREEMENT SHALL NOT EXCEED THE AMOUNT OF FEES (NOT INCLUDING MAINTENANCE PROGRAM CHARGES) PAID BY LICENSEE FOR USE OF THE SITECORE SOFTWARE UNDER THIS AGREEMENT. LICENSOR WILL NOT BE LIABLE FOR DELAYS OR FAILURES IN PERFORMANCE OF THE SUPPORT OR ANY OTHER SERVICES CAUSED BY FORCES BEYOND ITS CONTROL OR ANY FORCE MAJEURE EVENT SUCH AS ACT OF TERRORISM, LOSS OF POWER, ACT OF GOD, OR SIMILAR OCCURRENCE. THE LIMITATION PROVISIONS OF THIS SECTION SHALL BE APPLICABLE TO ANY CLAIM FILED BY LICENSEE ARISING OUT OF OR RELATING TO ANY SEPARATELY LICENSED SOFTWARE THAT MAY BE USED WITH THE SITECORE SOFTWARE.

5. Sitecore Maintenance Program : The Sitecore Maintenance Program includes Upgrades and support.  Upgrades are free of charge for as long as Licensee is enrolled in the Maintenance Program.  Licensee will automatically be included in the Maintenance Program, which is free of charge for the first year after the initial purchase of Sitecore Software. Licensee will subsequently be charged an annual fee of 20% of the retail price of all software purchased (the “Maintenance Program Charge”). Three years after the date of the initial purchase and each three-year anniversary thereafter, Licensor may adjust the Maintenance Program Charge then in effect for all software purchased under this Agreement to 20% of the Licensor’s then-current retail prices. Licensee may cancel the Maintenance Program by providing Licensor with 60 days notice prior to the end of any 12-month Maintenance Program period.  Licensor may cancel the Maintenance Program upon Licensee’s failure to pay timely any applicable Maintenance Program charge. A cancelled Maintenance Program may not be renewed except upon payment of double the amount of fees that would have been paid during the lapsed period. 
 
6. Upgrades and Patches of Sitecore Software : “Patch” as used in this Agreement means a specific, targeted fix to a discrete problem in the use or functionality of the Sitecore Software that Licensor in its sole discretion defines to constitute a “Patch” and may from time to time provide to Licensee. “Upgrade” as used in this Agreement means a new version of some or all of the Sitecore Software, or an improvement in the use or functionality of the Sitecore Software more substantial than a Patch, that Licensor in its sole discretion defines to constitute an “Upgrade” and may from time to time provide to Licensees enrolled in the Sitecore Maintenance program. Licensee expressly acknowledges that Upgrades and Patches may change functionality of the Sitecore Software and integration with other systems, and may not work with some or all of the Sitecore Software modules, or be backward compatible with earlier versions of Sitecore Software. Installation of Patches and Upgrades is the choice and responsibility of Licensee. To the extent that operation of the Sitecore Software is affected by problems in standard software, including, but not limited to, Microsoft Internet Explorer, Windows and the Microsoft.Net Framework, then Licensee shall install updates to such standard software per Licensor’s specifications in order to resolve these issues. Licensor has no control over such standard software, and cannot assure that problems with such standard software will be corrected, or that such corrections will be made in a timely manner.

7.  Waivers : All waivers must be in writing and signed by authorized representatives of the parties. Any waiver or failure to enforce any provision of this Agreement on one occasion shall not be deemed a waiver of any other provision or of such provision on any other occasion.

8.  Severability : If any provision of this Agreement is adjudicated to be unenforceable, such provision shall be deemed changed and interpreted to accomplish the objectives of such provision to the greatest extent possible under applicable law and the remaining provisions shall continue in full force and effect.

9.  Assignment : Licensee may not assign this Agreement, by operation of law or otherwise, which includes any change of control in the ownership structure of Licensee. Licensor may assign this Agreement to a successor (whether by merger, a sale of all or a significant portion of its assets, a sale of a controlling interest of its capital stock, or otherwise) that agrees to assume Licensor’s obligations under this Agreement. Any attempted assignment or transfer in violation of this Section shall be void and of no force or effect. Subject to the provisions of this Section 9, this Agreement shall be binding upon the successors and assigns of the parties.

10. Entire Agreement : This Agreement, and any attachment that is expressly incorporated in this Agreement, constitutes the entire agreement between the parties regarding the subject matter hereof and supersedes all prior or contemporaneous agreements, understandings and communications, whether written or oral. This Agreement may be amended only by a written document signed by both parties. In the event of a conflict between any provision of this Agreement with any other attachment or document, this Agreement shall control. Any term or condition not specifically authorized by this Agreement included in any Licensee invoice, purchase order or other document rendered pursuant to this Agreement is of no force or effect unless the specific term or condition has been previously agreed to by the parties in writing in a separate agreement. No action by Licensor (including, without limitation, receipt of payment of any such invoice, or acceptance of any purchase order, in whole or in part) shall be construed as making any such term or condition binding on Licensor.

By clicking 'Accept' you accept the License Agreement."" />
      </steps>
    </agreement>
    <setup title=""Initial Configuration Wizard"" startButton=""Next""
           finishText=""Congratulations! The installation was successfully completed and you can start using it out of the box. If you don't have any Sitecore zip files in the local repository then you may download them from SDN via Download Sitecores from SDN button on the Ribbon or do it manually""
           cancelButton=""Exit"">
      <steps afterLastStep=""SIM.Tool.Windows.Pipelines.Setup.SetupProcessor, SIM.Tool.Windows"">
        <step name=""Welcome message"" type=""SIM.Tool.Windows.UserControls.ConfirmStepUserControl, SIM.Tool.Windows""
              param=""PLEASE READ IT CAREFULLY! You can see this wizard because it is the first time Sitecore Instance Manager (SIM) was executed in this user account. You should accept license agreement and then set your preferences before you can use it, this wizard will help you.
              
Before you being please make sure that you have an IIS 7.x and SQL Server 2008+ installed on your PC and you have access to them. 
              
PLEASE NOTE that all your application settings and log files are stored in your personal folder (%APPDATA%\Sitecore\Sitecore Instance Manager) so that other users of this PC will not see, use or change your setting details. "" />
        <step name=""License agreement from marketplace.sitecore.net""
              type=""SIM.Tool.Windows.UserControls.ConfirmStepUserControl, SIM.Tool.Windows""
              param=""YOU SHOULD CAREFULLY READ THE FOLLOWING TERMS AND CONDITIONS BEFORE USING THIS SOFTWARE (THE “SITECORE SOFTWARE”). ANY USE OF THE SITECORE SOFTWARE IS SUBJECT TO YOUR FULL ACCEPTANCE OF THIS LICENSE AGREEMENT.

Sitecore License Agreement

LICENSEE’S USE OF THE SITECORE SOFTWARE IS SUBJECT TO LICENSEE’S FULL ACCEPTANCE OF THE TERMS, CONDITIONS, DISCLAIMERS AND LICENSE RESTRICTIONS SET FORTH IN THIS AGREEMENT.

1. License Grant : Upon payment in full of the license fee, Licensor grants Licensee a non-exclusive, perpetual, non-transferable, non-assignable, non-sublicensable license, without time limitations, to use the Sitecore Software in supported configurations as described in the Documentation, in compliance with all applicable laws, in object code form only, exclusively for management of Licensee’s own current and future web infrastructure , subject to the terms and conditions set forth in this Agreement. Except as expressly authorized by this Agreement, “Licensee” as used herein does not include any other entity or person, including any present or future subsidiary or affiliate of Licensee, or any entity or person owning any interest in Licensee at present or in the future. “Sitecore Software” means the software that is licensed by Licensor in this Agreement, and any future Upgrades and Patches, as those terms are defined in Section 6 of this Agreement, that the Licensee may receive in accordance with the terms of the Agreement. “Documentation” means the resources made available in the reference section of the Sitecore Developer Network setting forth the then-current functional, operational, and performance capabilities of the Sitecore Software (http://sdn.sitecore.net/documentation).
 
1.1 License Key : Licensee will be provided a License Key that gives Licensee access to the Sitecore Software. The Sitecore Software may be used only on the equipment with the features and limitations specified in the License Key. The License Key shall be time-limited until full payment of the license fee has been received by Licensor. The License Key may limit the number of installations of the Sitecore Software and the capacity of the Servers on which the Sitecore Software may be installed. For purposes of this Agreement, one Server is defined as a physical or virtual Server with a processing power for the web process equivalent to at most eight CPU cores. For example, one physical Server may contain eight single-core CPUs, four dual-core CPUs, two quad-core CPUs or one eight-core CPU. A Server with three or four quad-core CPUs would therefore count as two Servers. A virtual server’s processing power will be counted as the number of physical cores allocated or processor core equivalents allocated to the virtual server.  For example, one virtual server with two physical processor cores allocated, or with the equivalent of two simulated processor cores allocated, would be counted as the equivalent of two CPU cores and therefore one Server.

1.2 Intellectual Property Rights : The Sitecore Software includes the following patents: U.S. Patent Nos. 7,856,345 and 8,255,526. Ownership of the Sitecore Software, and all worldwide rights, title and interest in and to the Intellectual Property associated with the Sitecore Software shall remain solely and exclusively with Licensor or with third parties that license modules included with the Sitecore Software. Licensee shall retain intact all applicable Licensor copyright, patent and/or trademark notices on and in all copies of the Sitecore Software. All rights, title, and interest in Sitecore Software not expressly granted to Licensee in this Agreement are reserved by Licensor. “Intellectual Property” as used in this Agreement means any and all patents, copyrights, trademarks, service marks and trade names (registered and unregistered), trade secrets, know-how, inventions, licenses and all other proprietary rights throughout the world related to the authorship, origin, design, utility, process, manufacture, programming, functionality and operation of Sitecore Software and its Derivative Works.

1.3 Confidential Information :   The term “Confidential Information” shall include any information, whether tangible or intangible, including, but not limited to, techniques, discoveries, inventions, ideas, processes, software (in source or object code form), designs, technology, technical specifications, flow charts, procedures, formulas, concepts, any financial data, and all business and marketing plans and information, in each case which is maintained in confidence by the disclosing party (“Disclosing Party”) and disclosed to the other party (“Recipient”) hereunder. The failure by the Disclosing Party to designate any tangible or intangible information as Confidential Information shall not give Recipient the right to treat such information as free from the restrictions imposed by this Agreement if the circumstances would lead a reasonable person to believe that such information is Confidential Information. Confidential Information does not include information which Recipient documents (a) is now, or hereafter becomes, through no act or failure to act on the part of Recipient, generally known or available to the public; (b) was rightfully in Recipient’s possession prior to disclosure by the Disclosing Party; (c) becomes rightfully known to Recipient, without restriction, from a source other than the Disclosing Party and without any breach of duty to the Disclosing Party;  (d) is developed independently by Recipient without use of or reference to any of the Confidential Information and without violation of any confidentiality restriction contained herein; or (e) is approved by the Disclosing Party for disclosure without restriction, in a written document executed by a duly authorized officer of the Disclosing Party. Recipient shall hold the Confidential Information received from the Disclosing Party in strict confidence and shall not, directly or indirectly, disclose it, except as expressly permitted herein. Recipient shall promptly notify the Disclosing Party upon learning of any misappropriation or misuse of Confidential Information disclosed hereunder. Notwithstanding the foregoing, Recipient shall be permitted to disclose Confidential Information pursuant to a judicial or governmental order, provided that Recipient provides the Disclosing Party reasonable prior notice, and assistance, to contest such order.
 
1.4 Restrictions on Use : Except as expressly authorized by applicable law or by Licensor in writing, Licensee shall not copy, in whole or in part, the Sitecore Software or Documentation, or modify, disassemble, decompress, reverse compile, reverse assemble, reverse engineer, or translate any portion of the Sitecore Software. Licensee shall not rent, lease, lend, distribute, sell, assign, license, or otherwise transfer the Sitecore Software, or create Derivative Works of the Sitecore Software. Additionally, Licensee may use embedded third-party components in the Sitecore Software exclusively as described in the OEM section of the Sitecore Developer Network ( http://sdn.sitecore.net/oem ).

1.5  Derivative Works : “Derivative Works” as used herein means any software program (whether in source code or object code), and all copies thereof, developed by or on behalf of Licensee based on or derived from any part of the Sitecore Software, including without limitation any revision, modification, enhancement, translation (including compilation or recapitulation by computer), abridgment, condensation, expansion, or any other form in which the Sitecore Software may be recast, transformed or adapted, and that, if prepared without Licensor's authorization, would constitute a patent, copyright or trade secret infringement of the Sitecore Software, or would otherwise constitute an unauthorized use of Licensor’s Confidential Information. In the event any such Derivative Works are created, Licensor shall own all right, title, and interest in and to such Derivative Works. If, under the operation of local law or otherwise, Licensee or such third party comes to have any rights associated with such Derivative Works, Licensee hereby and shall automatically assign all such rights to Licensor for no additional consideration. For avoidance of doubt, Licensor shall claim no intellectual property interest or legal interest of any kind in any code created by Licensee to facilitate its authorized use of the Sitecore Software so long as the creation of such code does not constitute a Derivative Work or violate any other provision of this Agreement.

1.6 Third Party Use and Rights : Licensee may authorize third parties to assist Licensee in the management, editing, and hosting of web-based content created by use of the Sitecore Software, provided that: (1) such activities are within the scope of the activities Licensee is itself authorized to perform under this Agreement; (2) such third party’s acts are primarily for the direct or indirect benefit of Licensee; and (3) such third parties are not charged a fee by Licensee for such activities. Licensee is prohibited from using the Sitecore Software as an Application Software Provider or in any time-sharing or other commercial arrangement of any kind that makes the Sitecore Software available to third parties primarily for the third party’s own uses. Except as expressly stated in this Agreement, no third party has any rights under this Agreement. Licensee is fully liable to the extent allowed by law for any unauthorized use of the Sitecore Software by third parties caused by any acts or omissions of Licensee.

1.7 License Verification : The Sitecore Software periodically transmits the following information to a server maintained by Licensor: License Key ID, licensee name, hostname (Licensee’s website URL), host IP, Sitecore Software version, and directory installation path. This information is used by Licensor to send periodic communications to Licensee relevant to use of the Sitecore Software, and to verify, at Licensor’s election, that the Sitecore Software is installed in accordance with the terms of this Agreement.

 

2 . Limited Warranties :
 
2.1 Licensor’s Limited Warranty : Licensor expressly warrants that the Sitecore Software provided to Licensee will work for a period of ninety (90) days following the effective date of this Agreement according to the Documentation. In the event any such Sitecore Software does not operate according to the Documentation during this Limited Warranty Period, Licensor shall repair or replace the Sitecore Software and such repair or replacement shall be Licensee’s sole and exclusive remedy. 

2.2 Virus/Malicious Code Warranty : Licensor and Licensee warrant that they will use commercially reasonable virus and malicious code detection software programs to test any electronic files, including electronic communications, prior to any delivery or upon receiving such communications and that the parties will continue to take such steps with respect to exchanging electronic files and communications pursuant to this Agreement. In the event either party detects any computer virus or malicious code it shall immediately notify the other party and where possible shall promptly provide revised replacement files in the event any such computer virus or malicious code is detected.

3. DISCLAIMER OF WARRANTIES: EXCEPT AS EXPRESSLY SET FORTH IN SECTION 2 ABOVE, THE SITECORE SOFTWARE AND THE DOCUMENTATION ARE SUPPLIED TO LICENSEE 'AS IS.' LICENSOR (DEFINED IN THIS SECTION AS LICENSOR’S PARENT, AFFILIATES, SUBSIDIARIES, DISTRIBUTORS AND THEIR RESPECTIVE OFFICERS, DIRECTORS AND EMPLOYEES) MAKES NO WARRANTY, EXPRESS OR IMPLIED, AS TO THE SITECORE SOFTWARE, THE DOCUMENTATION, THE OPERATION OF THE SITECORE SOFTWARE, OR ANY OTHER GOODS OR SERVICES RENDERED BY LICENSOR TO LICENSEE, INCLUDING BUT NOT LIMITED TO, ANY IMPLIED WARRANTIES OF MERCHANTABILITY, PERFORMANCE, ACCURACY, NON-INFRINGEMENT, FITNESS FOR A PARTICULAR PURPOSE AND ALL WARRANTIES ARISING FROM COURSE OF DEALING OR USAGE OF TRADE, EXCEPT TO THE EXTENT ANY WARRANTIES IMPLIED BY LAW CANNOT BE WAIVED. LICENSOR EXPRESSLY DISCLAIMS ANY AND ALL EXPRESS, IMPLIED OR STATUTORY WARRANTIES THAT THE SITECORE SOFTWARE WILL RUN PROPERLY ON ANY HARDWARE, THAT THE SITECORE SOFTWARE WILL MEET LICENSEE’S REQUIREMENTS OR OPERATE IN THE COMBINATIONS WHICH MAY BE SELECTED FOR USE BY LICENSEE, OR THAT THE OPERATION OF THE SITECORE SOFTWARE WILL BE UNINTERRUPTED OR ERROR FREE, OR THAT ALL ERRORS WILL BE CORRECTED. LICENSOR MAKES NO EXPRESS OR IMPLIED WARRANTY OF ANY KIND REGARDING ANY SEPARATELY LICENSED SOFTWARE THAT MAY BE USED WITH THE SITECORE SOFTWARE.
 
4. LIMITATION OF LIABILITY: LICENSOR (DEFINED IN THIS SECTION AS LICENSOR’S PARENT, AFFILIATES, SUBSIDIARIES, DISTRIBUTORS AND THEIR RESPECTIVE OFFICERS, DIRECTORS AND EMPLOYEES) SHALL NOT BE LIABLE TO LICENSEE OR ANY THIRD PARTY FOR ANY INDIRECT, INCIDENTAL, SPECIAL, CONSEQUENTIAL, PUNITIVE, OR EXEMPLARY DAMAGES ARISING OUT OF OR RELATED TO THIS AGREEMENT UNDER ANY LEGAL THEORY, INCLUDING BUT NOT LIMITED TO, LOST PROFITS, LOST DATA OR BUSINESS INTERRUPTION, THE COST OF RECOVERING ANY DATA, INFRINGEMENT, OR THE COST OF SUBSTITUTE SOFTWARE, EVEN IF LICENSOR HAS BEEN ADVISED OF, KNOWS OF, OR SHOULD HAVE KNOWN, OF THE POSSIBILITY OF SUCH DAMAGES. LICENSOR’S AGGREGATE LIABILITY UNDER THIS AGREEMENT SHALL NOT EXCEED THE AMOUNT OF FEES (NOT INCLUDING MAINTENANCE PROGRAM CHARGES) PAID BY LICENSEE FOR USE OF THE SITECORE SOFTWARE UNDER THIS AGREEMENT. LICENSOR WILL NOT BE LIABLE FOR DELAYS OR FAILURES IN PERFORMANCE OF THE SUPPORT OR ANY OTHER SERVICES CAUSED BY FORCES BEYOND ITS CONTROL OR ANY FORCE MAJEURE EVENT SUCH AS ACT OF TERRORISM, LOSS OF POWER, ACT OF GOD, OR SIMILAR OCCURRENCE. THE LIMITATION PROVISIONS OF THIS SECTION SHALL BE APPLICABLE TO ANY CLAIM FILED BY LICENSEE ARISING OUT OF OR RELATING TO ANY SEPARATELY LICENSED SOFTWARE THAT MAY BE USED WITH THE SITECORE SOFTWARE.

5. Sitecore Maintenance Program : The Sitecore Maintenance Program includes Upgrades and support.  Upgrades are free of charge for as long as Licensee is enrolled in the Maintenance Program.  Licensee will automatically be included in the Maintenance Program, which is free of charge for the first year after the initial purchase of Sitecore Software. Licensee will subsequently be charged an annual fee of 20% of the retail price of all software purchased (the “Maintenance Program Charge”). Three years after the date of the initial purchase and each three-year anniversary thereafter, Licensor may adjust the Maintenance Program Charge then in effect for all software purchased under this Agreement to 20% of the Licensor’s then-current retail prices. Licensee may cancel the Maintenance Program by providing Licensor with 60 days notice prior to the end of any 12-month Maintenance Program period.  Licensor may cancel the Maintenance Program upon Licensee’s failure to pay timely any applicable Maintenance Program charge. A cancelled Maintenance Program may not be renewed except upon payment of double the amount of fees that would have been paid during the lapsed period. 
 
6. Upgrades and Patches of Sitecore Software : “Patch” as used in this Agreement means a specific, targeted fix to a discrete problem in the use or functionality of the Sitecore Software that Licensor in its sole discretion defines to constitute a “Patch” and may from time to time provide to Licensee. “Upgrade” as used in this Agreement means a new version of some or all of the Sitecore Software, or an improvement in the use or functionality of the Sitecore Software more substantial than a Patch, that Licensor in its sole discretion defines to constitute an “Upgrade” and may from time to time provide to Licensees enrolled in the Sitecore Maintenance program. Licensee expressly acknowledges that Upgrades and Patches may change functionality of the Sitecore Software and integration with other systems, and may not work with some or all of the Sitecore Software modules, or be backward compatible with earlier versions of Sitecore Software. Installation of Patches and Upgrades is the choice and responsibility of Licensee. To the extent that operation of the Sitecore Software is affected by problems in standard software, including, but not limited to, Microsoft Internet Explorer, Windows and the Microsoft.Net Framework, then Licensee shall install updates to such standard software per Licensor’s specifications in order to resolve these issues. Licensor has no control over such standard software, and cannot assure that problems with such standard software will be corrected, or that such corrections will be made in a timely manner.

7.  Waivers : All waivers must be in writing and signed by authorized representatives of the parties. Any waiver or failure to enforce any provision of this Agreement on one occasion shall not be deemed a waiver of any other provision or of such provision on any other occasion.

8.  Severability : If any provision of this Agreement is adjudicated to be unenforceable, such provision shall be deemed changed and interpreted to accomplish the objectives of such provision to the greatest extent possible under applicable law and the remaining provisions shall continue in full force and effect.

9.  Assignment : Licensee may not assign this Agreement, by operation of law or otherwise, which includes any change of control in the ownership structure of Licensee. Licensor may assign this Agreement to a successor (whether by merger, a sale of all or a significant portion of its assets, a sale of a controlling interest of its capital stock, or otherwise) that agrees to assume Licensor’s obligations under this Agreement. Any attempted assignment or transfer in violation of this Section shall be void and of no force or effect. Subject to the provisions of this Section 9, this Agreement shall be binding upon the successors and assigns of the parties.

10. Entire Agreement : This Agreement, and any attachment that is expressly incorporated in this Agreement, constitutes the entire agreement between the parties regarding the subject matter hereof and supersedes all prior or contemporaneous agreements, understandings and communications, whether written or oral. This Agreement may be amended only by a written document signed by both parties. In the event of a conflict between any provision of this Agreement with any other attachment or document, this Agreement shall control. Any term or condition not specifically authorized by this Agreement included in any Licensee invoice, purchase order or other document rendered pursuant to this Agreement is of no force or effect unless the specific term or condition has been previously agreed to by the parties in writing in a separate agreement. No action by Licensor (including, without limitation, receipt of payment of any such invoice, or acceptance of any purchase order, in whole or in part) shall be construed as making any such term or condition binding on Licensor.

By clicking 'Next' you accept the License Agreement."" />
        <step name=""Instances Root Folder"" type=""SIM.Tool.Windows.UserControls.Setup.InstancesRoot, SIM.Tool.Windows"" />
        <step name=""Local Repository and Sitecore License""
              type=""SIM.Tool.Windows.UserControls.Setup.LocalRepository, SIM.Tool.Windows"" />

        <step name=""SQL Server Connection String""
              type=""SIM.Tool.Windows.UserControls.Setup.ConnectionString, SIM.Tool.Windows"" />
        <step name=""File System permissions"" type=""SIM.Tool.Windows.UserControls.Setup.Permissions, SIM.Tool.Windows"" />
      </steps>
    </setup>

    <download8 title=""Download Sitecore Wizard"" startButton=""Next"" finishText=""Done"" cancelButton=""Exit"">
      <steps>
        <step name=""Welcome message"" type=""SIM.Tool.Windows.UserControls.ConfirmStepUserControl, SIM.Tool.Windows""
              param=""This wizard helps you to download packages ('ZIP archive of the Sitecore CMS site root') of the Sitecore CMS versions that Sitecore Instance Manager will be able to install for you. It will require your credentials from dev.sitecore.net because all downloads are protected from being downloaded by anonymous users. 
              
Note that due to the large size of each installation package the whole download operation may require much time. With a slow internet connection it may require up to 30 minutes per one Sitecore CMS version.

In addition, you may download these files from dev.sitecore.net to your local repository folder manually using your browser and/or any download program"" />
        <step name=""License agreement from dev.sitecore.net""
              type=""SIM.Tool.Windows.UserControls.ConfirmStepUserControl, SIM.Tool.Windows""
              param=""Please note that if you are an existing or new licensee or Sitecore Solution Partner with a valid agreement to use the Sitecore Software (“Existing Agreement”), the below license agreement is not intended to in any way modify or replace your Existing Agreement. If your Existing Agreement conflicts with the below license agreement, the terms of your Existing Agreement will prevail.  
 
Sitecore License Agreement
LICENSEE’S USE OF THE SITECORE SOFTWARE IS SUBJECT TO LICENSEE’S FULL ACCEPTANCE OF THE TERMS, CONDITIONS, DISCLAIMERS AND LICENSE RESTRICTIONS SET FORTH IN THIS AGREEMENT.
1. License Grant : Upon payment in full of the license fee, Licensor grants Licensee a non-exclusive, perpetual, non-transferable, non-assignable, non-sublicensable license, without time limitations, to use the Sitecore Software in supported configurations as described in the Documentation, in compliance with all applicable laws, in object code form only, exclusively for the Permitted Usage (as that term is defined in Exhibit A) , subject to the terms and conditions set forth in this Agreement and Exhibits A and B hereto, which are incorporated herein and made a part of this Agreement. Except as expressly authorized by this Agreement, “Licensee” as used herein does not include any other entity or person, including any present or future subsidiary or affiliate of Licensee, or any entity or person owning any interest in Licensee at present or in the future. “Sitecore Software” means the software that is licensed by Licensor in this Agreement, and any future Upgrades and Patches, as those terms are defined in Section 6 of this Agreement, that the Licensee may receive in accordance with the terms of the Agreement. “Documentation” means the resources made available in the reference section of the Sitecore Developer Network setting forth the then-current functional, operational, and performance capabilities of the Sitecore Software (http://sdn.sitecore.net/documentation).
 
1.1 License Key : Licensee will be provided a License Key that gives Licensee access to the Sitecore Software. The Sitecore Software may be used only on the equipment with the features and limitations specified in the License Key. The License Key shall be time-limited until full payment of the license fee has been received by Licensor. The License Key may limit the number of installations of the Sitecore Software and the capacity of the Servers on which the Sitecore Software may be installed. For purposes of this Agreement, one Server is defined as a physical or virtual Server with a processing power for the web process equivalent to at most eight CPU cores. For example, one physical Server may contain eight single-core CPUs, four dual-core CPUs, two quad-core CPUs or one eight-core CPU. A Server with three or four quad-core CPUs would therefore count as two Servers. A virtual server’s processing power will be counted as the number of physical cores allocated or processor core equivalents allocated to the virtual server.  For example, one virtual server with two physical processor cores allocated, or with the equivalent of two simulated processor cores allocated, would be counted as the equivalent of two CPU cores and therefore one Server.
1.2 Intellectual Property Rights : The Sitecore Software includes the following patents: U.S. Patent Nos. 7,856,345 and 8,255,526. Ownership of the Sitecore Software, and all worldwide rights, title and interest in and to the Intellectual Property associated with the Sitecore Software shall remain solely and exclusively with Licensor or with third parties that license modules included with the Sitecore Software. Licensee shall retain intact all applicable Licensor copyright, patent and/or trademark notices on and in all copies of the Sitecore Software. All rights, title, and interest in Sitecore Software not expressly granted to Licensee in this Agreement are reserved by Licensor. “Intellectual Property” as used in this Agreement means any and all patents, copyrights, trademarks, service marks and trade names (registered and unregistered), trade secrets, know-how, inventions, licenses and all other proprietary rights throughout the world related to the authorship, origin, design, utility, process, manufacture, programming, functionality and operation of Sitecore Software and its Derivative Works.
1.3 Confidential Information :   The term “Confidential Information” shall include any information, whether tangible or intangible, including, but not limited to, techniques, discoveries, inventions, ideas, processes, software (in source or object code form), designs, technology, technical specifications, flow charts, procedures, formulas, concepts, any financial data, and all business and marketing plans and information, in each case which is maintained in confidence by the disclosing party (“Disclosing Party”) and disclosed to the other party (“Recipient”) hereunder. The failure by the Disclosing Party to designate any tangible or intangible information as Confidential Information shall not give Recipient the right to treat such information as free from the restrictions imposed by this Agreement if the circumstances would lead a reasonable person to believe that such information is Confidential Information. Confidential Information does not include information which Recipient documents (a) is now, or hereafter becomes, through no act or failure to act on the part of Recipient, generally known or available to the public; (b) was rightfully in Recipient’s possession prior to disclosure by the Disclosing Party; (c) becomes rightfully known to Recipient, without restriction, from a source other than the Disclosing Party and without any breach of duty to the Disclosing Party;  (d) is developed independently by Recipient without use of or reference to any of the Confidential Information and without violation of any confidentiality restriction contained herein; or (e) is approved by the Disclosing Party for disclosure without restriction, in a written document executed by a duly authorized officer of the Disclosing Party. Recipient shall hold the Confidential Information received from the Disclosing Party in strict confidence and shall not, directly or indirectly, disclose it, except as expressly permitted herein. Recipient shall promptly notify the Disclosing Party upon learning of any misappropriation or misuse of Confidential Information disclosed hereunder. Notwithstanding the foregoing, Recipient shall be permitted to disclose Confidential Information pursuant to a judicial or governmental order, provided that Recipient provides the Disclosing Party reasonable prior notice, and assistance, to contest such order.
 
1.4 Restrictions on Use : Except as expressly authorized by applicable law or by Licensor in writing, Licensee shall not copy, in whole or in part, the Sitecore Software or Documentation, or modify, disassemble, decompress, reverse compile, reverse assemble, reverse engineer, or translate any portion of the Sitecore Software. Licensee shall not rent, lease, lend, distribute, sell, assign, license, or otherwise transfer the Sitecore Software, or create Derivative Works of the Sitecore Software. Additionally, Licensee may use embedded third-party components in the Sitecore Software exclusively as described in the OEM section of the Sitecore Developer Network ( http://sdn.sitecore.net/oem ).
1.5  Derivative Works : “Derivative Works” as used herein means any software program (whether in source code or object code), and all copies thereof, developed by or on behalf of Licensee based on or derived from any part of the Sitecore Software, including without limitation any revision, modification, enhancement, translation (including compilation or recapitulation by computer), abridgment, condensation, expansion, or any other form in which the Sitecore Software may be recast, transformed or adapted, and that, if prepared without Licensor's authorization, would constitute a patent, copyright or trade secret infringement of the Sitecore Software, or would otherwise constitute an unauthorized use of Licensor’s Confidential Information. In the event any such Derivative Works are created, Licensor shall own all right, title, and interest in and to such Derivative Works. If, under the operation of local law or otherwise, Licensee or such third party comes to have any rights associated with such Derivative Works, Licensee hereby and shall automatically assign all such rights to Licensor for no additional consideration. For avoidance of doubt, Licensor shall claim no intellectual property interest or legal interest of any kind in any code created by Licensee to facilitate its authorized use of the Sitecore Software so long as the creation of such code does not constitute a Derivative Work or violate any other provision of this Agreement.
1.6 Third Party Use and Rights : Licensee may authorize third parties to assist Licensee in the management, editing, and hosting of web-based content created by use of the Sitecore Software, provided that: (1) such activities are within the scope of the activities Licensee is itself authorized to perform under this Agreement; (2) such third party’s acts are primarily for the direct or indirect benefit of Licensee; and (3) such third parties are not charged a fee by Licensee for such activities. Licensee is prohibited from using the Sitecore Software as an Application Software Provider or in any time-sharing or other commercial arrangement of any kind that makes the Sitecore Software available to third parties primarily for the third party’s own uses. Except as expressly stated in this Agreement, no third party has any rights under this Agreement. Licensee is fully liable to the extent allowed by law for any unauthorized use of the Sitecore Software by third parties caused by any acts or omissions of Licensee.
1.7 License Verification : The Sitecore Software periodically transmits the following information to a server maintained by Licensor: License Key ID, licensee name, hostname (Licensee’s website URL), host IP, Sitecore Software version, and directory installation path. This information is used by Licensor to send periodic communications to Licensee relevant to use of the Sitecore Software, and to verify, at Licensor’s election, that the Sitecore Software is installed in accordance with the terms of this Agreement.
 
2 . Limited Warranties :
 
2.1 Licensor’s Limited Warranty : Licensor expressly warrants that the Sitecore Software provided to Licensee will work for a period of ninety (90) days following the effective date of this Agreement according to the Documentation. In the event any such Sitecore Software does not operate according to the Documentation during this Limited Warranty Period, Licensor shall repair or replace the Sitecore Software and such repair or replacement shall be Licensee’s sole and exclusive remedy. 
2.2 Virus/Malicious Code Warranty : Licensor and Licensee warrant that they will use commercially reasonable virus and malicious code detection software programs to test any electronic files, including electronic communications, prior to any delivery or upon receiving such communications and that the parties will continue to take such steps with respect to exchanging electronic files and communications pursuant to this Agreement. In the event either party detects any computer virus or malicious code it shall immediately notify the other party and where possible shall promptly provide revised replacement files in the event any such computer virus or malicious code is detected.
3. DISCLAIMER OF WARRANTIES: EXCEPT AS EXPRESSLY SET FORTH IN SECTION 2 ABOVE, THE SITECORE SOFTWARE AND THE DOCUMENTATION ARE SUPPLIED TO LICENSEE 'AS IS.' LICENSOR (DEFINED IN THIS SECTION AS LICENSOR’S PARENT, AFFILIATES, SUBSIDIARIES, DISTRIBUTORS AND THEIR RESPECTIVE OFFICERS, DIRECTORS AND EMPLOYEES) MAKES NO WARRANTY, EXPRESS OR IMPLIED, AS TO THE SITECORE SOFTWARE, THE DOCUMENTATION, THE OPERATION OF THE SITECORE SOFTWARE, OR ANY OTHER GOODS OR SERVICES RENDERED BY LICENSOR TO LICENSEE, INCLUDING BUT NOT LIMITED TO, ANY IMPLIED WARRANTIES OF MERCHANTABILITY, PERFORMANCE, ACCURACY, NON-INFRINGEMENT, FITNESS FOR A PARTICULAR PURPOSE AND ALL WARRANTIES ARISING FROM COURSE OF DEALING OR USAGE OF TRADE, EXCEPT TO THE EXTENT ANY WARRANTIES IMPLIED BY LAW CANNOT BE WAIVED. LICENSOR EXPRESSLY DISCLAIMS ANY AND ALL EXPRESS, IMPLIED OR STATUTORY WARRANTIES THAT THE SITECORE SOFTWARE WILL RUN PROPERLY ON ANY HARDWARE, THAT THE SITECORE SOFTWARE WILL MEET LICENSEE’S REQUIREMENTS OR OPERATE IN THE COMBINATIONS WHICH MAY BE SELECTED FOR USE BY LICENSEE, OR THAT THE OPERATION OF THE SITECORE SOFTWARE WILL BE UNINTERRUPTED OR ERROR FREE, OR THAT ALL ERRORS WILL BE CORRECTED. LICENSOR MAKES NO EXPRESS OR IMPLIED WARRANTY OF ANY KIND REGARDING ANY SEPARATELY LICENSED SOFTWARE THAT MAY BE USED WITH THE SITECORE SOFTWARE.
 
4. LIMITATION OF LIABILITY: LICENSOR (DEFINED IN THIS SECTION AS LICENSOR’S PARENT, AFFILIATES, SUBSIDIARIES, DISTRIBUTORS AND THEIR RESPECTIVE OFFICERS, DIRECTORS AND EMPLOYEES) SHALL NOT BE LIABLE TO LICENSEE OR ANY THIRD PARTY FOR ANY INDIRECT, INCIDENTAL, SPECIAL, CONSEQUENTIAL, PUNITIVE, OR EXEMPLARY DAMAGES ARISING OUT OF OR RELATED TO THIS AGREEMENT UNDER ANY LEGAL THEORY, INCLUDING BUT NOT LIMITED TO, LOST PROFITS, LOST DATA OR BUSINESS INTERRUPTION, THE COST OF RECOVERING ANY DATA, INFRINGEMENT, OR THE COST OF SUBSTITUTE SOFTWARE, EVEN IF LICENSOR HAS BEEN ADVISED OF, KNOWS OF, OR SHOULD HAVE KNOWN, OF THE POSSIBILITY OF SUCH DAMAGES. LICENSOR’S AGGREGATE LIABILITY UNDER THIS AGREEMENT SHALL NOT EXCEED THE AMOUNT OF FEES (NOT INCLUDING MAINTENANCE PROGRAM CHARGES) PAID BY LICENSEE FOR USE OF THE SITECORE SOFTWARE UNDER THIS AGREEMENT. LICENSOR WILL NOT BE LIABLE FOR DELAYS OR FAILURES IN PERFORMANCE OF THE SUPPORT OR ANY OTHER SERVICES CAUSED BY FORCES BEYOND ITS CONTROL OR ANY FORCE MAJEURE EVENT SUCH AS ACT OF TERRORISM, LOSS OF POWER, ACT OF GOD, OR SIMILAR OCCURRENCE. THE LIMITATION PROVISIONS OF THIS SECTION SHALL BE APPLICABLE TO ANY CLAIM FILED BY LICENSEE ARISING OUT OF OR RELATING TO ANY SEPARATELY LICENSED SOFTWARE THAT MAY BE USED WITH THE SITECORE SOFTWARE.
5. Sitecore Maintenance Program : Licensee will automatically be enrolled in the Sitecore Maintenance Program. For each Maintenance Program Period (as that term is defined in Exhibit A) Licensee will be charged a non-refundable Maintenance Program Charge (as that term is defined in Exhibit A). The Sitecore Maintenance Program includes Upgrades and support. Upgrades are free of charge for as long as Licensee is enrolled in the Maintenance Program. Three years after the date of the initial purchase and each three-year anniversary thereafter, Licensor may adjust the Maintenance Program Charge then in effect for all software purchased under this Agreement to 20% of the Licensor’s then-current retail prices. Licensee may cancel the Maintenance Program by providing Licensor with 60 days notice prior to the end of any Maintenance Program Period. Licensor may cancel the Maintenance Program upon Licensee’s failure to pay timely any applicable Maintenance Program Charge. A cancelled Maintenance Program may not be renewed except upon payment of double the amount of fees that would have been paid during the lapsed period. 
 
6. Upgrades and Patches of Sitecore Software : “Patch” as used in this Agreement means a specific, targeted fix to a discrete problem in the use or functionality of the Sitecore Software that Licensor in its sole discretion defines to constitute a “Patch” and may from time to time provide to Licensee. “Upgrade” as used in this Agreement means a new version of some or all of the Sitecore Software, or an improvement in the use or functionality of the Sitecore Software more substantial than a Patch, that Licensor in its sole discretion defines to constitute an “Upgrade” and may from time to time provide to Licensees enrolled in the Sitecore Maintenance program. Licensee expressly acknowledges that Upgrades and Patches may change functionality of the Sitecore Software and integration with other systems, and may not work with some or all of the Sitecore Software modules, or be backward compatible with earlier versions of Sitecore Software. Installation of Patches and Upgrades is the choice and responsibility of Licensee. To the extent that operation of the Sitecore Software is affected by problems in standard software, including, but not limited to, Microsoft Internet Explorer, Windows and the Microsoft.Net Framework, then Licensee shall install updates to such standard software per Licensor’s specifications in order to resolve these issues. Licensor has no control over such standard software, and cannot assure that problems with such standard software will be corrected, or that such corrections will be made in a timely manner.
7.  Waivers : All waivers must be in writing and signed by authorized representatives of the parties. Any waiver or failure to enforce any provision of this Agreement on one occasion shall not be deemed a waiver of any other provision or of such provision on any other occasion.
8.  Severability : If any provision of this Agreement is adjudicated to be unenforceable, such provision shall be deemed changed and interpreted to accomplish the objectives of such provision to the greatest extent possible under applicable law and the remaining provisions shall continue in full force and effect.
9.  Assignment : Licensee may not assign this Agreement, by operation of law or otherwise, which includes any change of control in the ownership structure of Licensee. Licensor may assign this Agreement to a successor (whether by merger, a sale of all or a significant portion of its assets, a sale of a controlling interest of its capital stock, or otherwise) that agrees to assume Licensor’s obligations under this Agreement. Any attempted assignment or transfer in violation of this Section shall be void and of no force or effect. Subject to the provisions of this Section 9, this Agreement shall be binding upon the successors and assigns of the parties.
10. Entire Agreement : This Agreement, and any attachment that is expressly incorporated in this Agreement, constitutes the entire agreement between the parties regarding the subject matter hereof and supersedes all prior or contemporaneous agreements, understandings and communications, whether written or oral. This Agreement may be amended only by a written document signed by both parties. In the event of a conflict between any provision of this Agreement with any other attachment or document, this Agreement shall control. Any term or condition not specifically authorized by this Agreement included in any Licensee invoice, purchase order or other document rendered pursuant to this Agreement is of no force or effect unless the specific term or condition has been previously agreed to by the parties in writing in a separate agreement. No action by Licensor (including, without limitation, receipt of payment of any such invoice, or acceptance of any purchase order, in whole or in part) shall be construed as making any such term or condition binding on Licensor.
 
Sitecore License Agreement. July 1, 2013 

By clicking 'Next' you accept the License Agreement."" />
        <step name=""Choose versions to download""
              type=""SIM.Tool.Windows.UserControls.Download8.Downloads, SIM.Tool.Windows"" />
        <step name=""Provide your dev.sitecore.net credentials""
              type=""SIM.Tool.Windows.UserControls.Download8.Login, SIM.Tool.Windows""
              param=""The following credentials will be used for authenticating in dev.sitecore.net and performing downloading selected Sitecore versions on behalf of you. "" />
      </steps>
    </download8>

<delete9 title=""Deleting instances"" startButton=""Delete""
             finishText=""The uninstallation was successfully completed"">
      <args type=""SIM.Tool.Base.Pipelines.Install9WizardArgs, SIM.Tool.Base""/>
      <steps>
        <step name=""STEP 1 of 2 - DETAILS"" 
              type=""SIM.Tool.Windows.UserControls.Install.Delete9Details, SIM.Tool.Windows"" />
        <step name=""STEP 2 of 2 - SELECT UNINSTALLATION TASKS"" 
              type=""SIM.Tool.Windows.UserControls.Install.Instance9SelectTasks, SIM.Tool.Windows"" />        
     </steps>     
     <finish>
         <hive type=""SIM.Tool.Windows.Pipelines.Install.Install9ActionsHive, SIM.Tool.Windows"" />
     </finish>
    </delete9>
    <reinstall9 title=""Reinstalling {InstanceName}"" startButton=""Reinstall""
             finishText=""The re-installation was successfully completed"">    
      <args type=""SIM.Tool.Base.Pipelines.ReinstallWizardArgs, SIM.Tool.Base""/>
      <steps>
        <step name=""Confirmation"" 
              type=""SIM.Tool.Windows.UserControls.Reinstall.Reinstall9Confirmation, SIM.Tool.Windows"" />       
     </steps>  
    </reinstall9>
    <install9 title=""Installing new instance"" startButton=""Install""
             finishText=""The installation was successfully completed"">
      <args type=""SIM.Tool.Base.Pipelines.Install9WizardArgs, SIM.Tool.Base""/>
      <steps>
        <step name=""STEP 1 of 2 - DETAILS"" 
              type=""SIM.Tool.Windows.UserControls.Install.Instance9Details, SIM.Tool.Windows"" />
        <step name=""STEP 2 of 2 - SELECT INSTALLATION TASKS"" 
              type=""SIM.Tool.Windows.UserControls.Install.Instance9SelectTasks, SIM.Tool.Windows"" />   
     </steps>   
     <finish>
         <hive type=""SIM.Tool.Windows.Pipelines.Install.Install9ActionsHive, SIM.Tool.Windows"" />
     </finish>
    </install9>
    <install title=""Installing new instance"" startButton=""Install""
             finishText=""The installation was successfully completed"">
      <steps>
        <step name=""STEP 1 of 7 - DETAILS"" 
              type=""SIM.Tool.Windows.UserControls.Install.InstanceDetails, SIM.Tool.Windows"" />
        <step name=""STEP 2 of 7 - TWEAKS"" 
              type=""SIM.Tool.Windows.UserControls.Install.InstanceSettings, SIM.Tool.Windows"" />
        <step name=""STEP 3 of 7 - CONFIGURATION ROLES"" 
              type=""SIM.Tool.Windows.UserControls.Install.InstanceRole, SIM.Tool.Windows"" />
        <step name=""STEP 3 of 7 - CONFIGURATION ROLES"" 
              type=""SIM.Tool.Windows.UserControls.Install.InstanceRole9, SIM.Tool.Windows"" />
        <step name=""STEP 4 of 7 - OFFICIAL SITECORE MODULES"" 
              type=""SIM.Tool.Windows.UserControls.Install.Modules.ModulesDetails, SIM.Tool.Windows"" />
        <step name=""STEP 5 of 7 - CUSTOM PACKAGES AND ZIP FILES""
              type=""SIM.Tool.Windows.UserControls.Install.Modules.FilePackages, SIM.Tool.Windows"" />
        <step name=""STEP 6 of 7 - CONFIGURATION PRESETS""
              type=""SIM.Tool.Windows.UserControls.Install.Modules.ConfigurationPackages, SIM.Tool.Windows"" />
        <step name=""STEP 7 of 7 - REVIEW, REARRANGE INSTALLATION ORDER OR ADD CUSTOM PACKAGE""
          type=""SIM.Tool.Windows.UserControls.Install.Modules.ReorderPackages, SIM.Tool.Windows"" />
      </steps>
      <finish>
        <action text=""Open in Browser"" type=""SIM.Tool.Windows.Pipelines.Install.InstallActions, SIM.Tool.Windows""
                method=""OpenBrowser"" />
        <action text=""Open in Browser (Sitecore Client)""
                type=""SIM.Tool.Windows.Pipelines.Install.InstallActions, SIM.Tool.Windows"" method=""OpenSitecoreClient"" />
        <action text=""Open in Browser (Sitecore Client; Log in as Admin)""
                type=""SIM.Tool.Windows.Pipelines.Install.InstallActions, SIM.Tool.Windows"" method=""LoginAdmin"" />
        <action text=""Open folder"" type=""SIM.Tool.Windows.Pipelines.Install.InstallActions, SIM.Tool.Windows""
                method=""OpenWebsiteFolder"" />
        <action text=""Open Visual Studio"" type=""SIM.Tool.Windows.Pipelines.Install.InstallActions, SIM.Tool.Windows""
                method=""OpenVisualStudio"" />
        <action text=""Make a back up"" type=""SIM.Tool.Windows.Pipelines.Install.InstallActions, SIM.Tool.Windows""
                method=""BackupInstance"" />
        <action text=""Publish Site"" type=""SIM.Tool.Windows.Pipelines.Install.InstallActions, SIM.Tool.Windows"" method=""PublishSite"" />
        <hive type=""SIM.Tool.Windows.Pipelines.Install.InstallModulesFinishActionHive, SIM.Tool.Windows"" />
      </finish>
    </install>
    <delete title=""Deleting the {InstanceName} instance"" startButton=""Delete""
            finishText=""The deleting was successfully completed"">
      <steps>
        <step name=""Information"" type=""SIM.Tool.Windows.UserControls.ConfirmStepUserControl, SIM.Tool.Windows""
              param=""The selected Sitecore instance will be deleted. These items will be deleted automatically:               
    • the root folder
    • the databases located inside the instance's root folder
    • the IIS website and application pool
    • the appropriate record in the hosts file

But the confirmation will be required if the databases are attached to:
    • the local SQL Server* and located out of the instance root folder        
    • any other SQL Server instance

* - the SQL Server instance specified by connection string in the Settings dialog"" />
      </steps>
    </delete>
    <multipleDeletion title=""Multiple deletion"" startButton=""Delete""
                      finishText=""The deleting was successfully completed"">
      <steps>
        <step name=""Information"" type=""SIM.Tool.Windows.UserControls.ConfirmStepUserControl, SIM.Tool.Windows""
              param=""These items will be deleted automatically for each of selected instances:               
    
    • the root folder
    • the databases located inside the instance's root folder
    • the IIS website and application pool
    • the appropriate record in the hosts file

But the confirmation will be required if the databases are attached to:

    • the local SQL Server* and located out of the instance root folder        
    • any other SQL Server instance

* - the SQL Server instance specified by connection string in the Settings dialog"" />
        <step name=""Select the instances that you want to delete""
              type=""SIM.Tool.Windows.UserControls.MultipleDeletion.SelectInstances, SIM.Tool.Windows"" />
        <step name=""Confirmation"" type=""SIM.Tool.Windows.UserControls.ConfirmStepUserControl, SIM.Tool.Windows""
              param=""Are you sure you want to delete the selected instances?"" />
      </steps>
    </multipleDeletion>
    <backup title=""Backing up the {InstanceName} instance"" startButton=""Backup""
            finishText=""The backup was successfully created"">
      <steps>
        <step name=""Information"" type=""SIM.Tool.Windows.UserControls.ConfirmStepUserControl, SIM.Tool.Windows""
              param=""A back up of the selected instance will be created. "" />
        <step name=""Specify the backup name and and select the necessary resources""
              type=""SIM.Tool.Windows.UserControls.Backup.BackupSettings, SIM.Tool.Windows"" />
      </steps>
    </backup>
    <restore title=""Restoring up the {InstanceName} instance"" startButton=""Restore""
             finishText=""The instance was successfully restored from the backup"">
      <steps>
        <step name=""Choose backup"" type=""SIM.Tool.Windows.UserControls.Backup.ChooseBackup, SIM.Tool.Windows"" />
      </steps>
    </restore>
    <export title=""Exporting the {InstanceName} instance"" startButton=""Export""
            finishText=""The export was successfully performed"">
      <steps>
        <step name=""Information"" type=""SIM.Tool.Windows.UserControls.ConfirmStepUserControl, SIM.Tool.Windows""
              param=""An export of the selected instance will be performed:
							
• exporting databases
• exporting files (Data and WebRoot folders)
• exporting instance settings
• assembling zip package"" />
        <step name=""Choose the databases for exporting""
              type=""SIM.Tool.Windows.UserControls.Export.ExportDatabases, SIM.Tool.Windows"" />
        <step name=""Specify path and name for the export file""
              type=""SIM.Tool.Windows.UserControls.Export.ExportFile, SIM.Tool.Windows"" />
      </steps>
      <finish>
        <action text=""Open folder"" type=""SIM.Tool.Windows.UserControls.Export.FinishActions, SIM.Tool.Windows""
                method=""OpenExportFolder"" />
      </finish>
    </export>
    <import title=""Importing Sitecore instance"" startButton=""Import"" finishText=""The import was successfully performed"">
      <steps>
        <step name=""Information"" type=""SIM.Tool.Windows.UserControls.ConfirmStepUserControl, SIM.Tool.Windows""
              param=""An import of the Sitecore instance will be performed:
							
• importing databases
• importing files (Data and WebRoot folders)
• moving your license to the Data
• importing IIS website and Application Pool "" />
        <step name=""Specify root path and website name""
              type=""SIM.Tool.Windows.UserControls.Import.ImportWebsite, SIM.Tool.Windows"" />
        <step name=""Change site bindings if needed""
              type=""SIM.Tool.Windows.UserControls.Import.SetWebsiteBindings, SIM.Tool.Windows"" />
      </steps>
    </import>
    <reinstall title=""Reinstalling the {InstanceName} instance"" startButton=""Reinstall""
               finishText=""The reinstalling was successfully completed"">
      <steps>
        <step name=""Information"" type=""SIM.Tool.Windows.UserControls.ConfirmStepUserControl, SIM.Tool.Windows""
              param=""The selected Sitecore instance will be re-installed without any modules. 
        
These items will be deleted automatically: 
    • the root folder
    • the databases located inside the instance's root folder
    • the IIS website and application pool
    • the appropriate record in the hosts file

But the confirmation will be required if the databases are attached to:
    • the local SQL Server* and located out of the instance root folder        
    • any other SQL Server instance

* - the SQL Server instance specified by connection string in the Settings dialog"" />
      </steps>
      <finish>
        <action text=""Open in Browser"" 
                type=""SIM.Tool.Windows.Pipelines.Reinstall.FinishActions, SIM.Tool.Windows"" method=""OpenBrowser"" />

        <action text=""Open in Browser (Sitecore Client)""
                type=""SIM.Tool.Windows.Pipelines.Reinstall.FinishActions, SIM.Tool.Windows"" method=""OpenSitecoreClient"" />

        <action text=""Open in Browser (Sitecore Client; Login as Admin)""
                type=""SIM.Tool.Windows.Pipelines.Reinstall.FinishActions, SIM.Tool.Windows"" method=""LoginAdmin"" />

        <action text=""Open folder"" 
                type=""SIM.Tool.Windows.Pipelines.Reinstall.FinishActions, SIM.Tool.Windows"" method=""OpenWebsiteFolder"" />

        <action text=""Open Visual Studio"" 
                type=""SIM.Tool.Windows.Pipelines.Reinstall.FinishActions, SIM.Tool.Windows"" method=""OpenVisualStudio"" />

        <action text=""Make a back up"" 
                type=""SIM.Tool.Windows.Pipelines.Reinstall.FinishActions, SIM.Tool.Windows"" method=""BackupInstance"" />
      </finish>
    </reinstall>
    <installmodules title=""Installing modules to the {InstanceName} instance"" startButton=""Install""
                    finishText=""The modules installation was successfully completed"">
      <steps>
        <step name=""STEP 1 of 4 - OFFICIAL SITECORE MODULES"" 
              type=""SIM.Tool.Windows.UserControls.Install.Modules.ModulesDetails, SIM.Tool.Windows"" />
        <step name=""STEP 2 of 4 - CUSTOM PACKAGES AND ZIP FILES""
              type=""SIM.Tool.Windows.UserControls.Install.Modules.FilePackages, SIM.Tool.Windows"" />
        <step name=""STEP 3 of 4 - CONFIGURATION PRESETS""
              type=""SIM.Tool.Windows.UserControls.Install.Modules.ConfigurationPackages, SIM.Tool.Windows"" />
        <step name=""STEP 4 of 4 - REVIEW, REARRANGE INSTALLATION ORDER OR ADD CUSTOM PACKAGE""
          type=""SIM.Tool.Windows.UserControls.Install.Modules.ReorderPackages, SIM.Tool.Windows"" />
      </steps>
      <finish>
        <action text=""Open in Browser"" type=""SIM.Tool.Windows.Pipelines.Install.InstallModulesActions, SIM.Tool.Windows""
                method=""OpenBrowser"" />
        <action text=""Open in Browser (Sitecore Client)""
                type=""SIM.Tool.Windows.Pipelines.Install.InstallModulesActions, SIM.Tool.Windows"" method=""OpenSitecoreClient"" />
        <action text=""Open in Browser (Sitecore Client; Login as Admin)""
                type=""SIM.Tool.Windows.Pipelines.Install.InstallModulesActions, SIM.Tool.Windows"" method=""LoginAdmin"" />
        <action text=""Open folder"" type=""SIM.Tool.Windows.Pipelines.Install.InstallModulesActions, SIM.Tool.Windows""
                method=""OpenWebsiteFolder"" />
        <action text=""Open Visual Studio"" type=""SIM.Tool.Windows.Pipelines.Install.InstallModulesActions, SIM.Tool.Windows""
                method=""OpenVisualStudio"" />
        <action text=""Make a back up"" type=""SIM.Tool.Windows.Pipelines.Install.InstallModulesActions, SIM.Tool.Windows""
                method=""BackupInstance"" />
        <action text=""Publish Site"" type=""SIM.Tool.Windows.Pipelines.Install.InstallModulesActions, SIM.Tool.Windows"" method=""PublishSite"" />
        <hive type=""SIM.Tool.Windows.Pipelines.Install.InstallModulesFinishActionHive, SIM.Tool.Windows"" />
      </finish>
    </installmodules>
  </wizardPipelines>
</configuration>
";
  }
}
