<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
 <xsl:template match="/">
  <html> 
   <body>  
    <h2>My Applications Details</h2> 
    <table border="1"><tr bgcolor="#9acd32"> 
	  <th>Window Title</th> 
      <th>Process Name</th> 
	  <th>Application Start Time</th>
	  <th>Application Stop Time</th>
      <th>Sequence Start Time</th>
      <th>Sequence Stop Time</th>
	  <th>Total Active Time</th> 
      </tr> 
      <xsl:for-each select="ApplDetails/Application_Info">
      <xsl:sort select="ApplicationName"/> 
	    <tr> 
          <td><xsl:value-of select="ProcessName"/></td> 
          <td><xsl:value-of select="ApplicationName"/></td>
          <td><xsl:value-of select="StartTime"/></td> 
		  <td><xsl:value-of select="ApplStopTime"/></td>
		  <td><xsl:value-of select="SeqStartTime"/></td>
          <td><xsl:value-of select="StopTime"/></td>
          <td><xsl:value-of select="TotalSeconds"/></td>
	    </tr> 
	  </xsl:for-each>  
    </table>
   </body> 
  </html>
 </xsl:template>
</xsl:stylesheet>
