<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:template match="/">
  <html>
  <body>
    <h2>My Applications Details</h2>
    <table border="1">
      <tr bgcolor="#9acd32">
        <th>Process Name</th>
        <th>Window Title</th>
        <th>Total Time</th>

      </tr>
      <xsl:for-each select="ApplDetails/Application_Info">
      <tr>
        <td><xsl:value-of select="ProcessName"/></td>
        <td><xsl:value-of select="ApplicationName"/></td>
        <td><xsl:value-of select="TotalSeconds"/></td>
      </tr>
      </xsl:for-each>
    </table>
  </body>
  </html>
</xsl:template>

</xsl:stylesheet>