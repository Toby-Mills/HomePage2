<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<rss>
			<channel>
				<xsl:for-each select="/rss/channel/item">
					<xsl:sort select="pubDate"/>
					<item>
						<link>
						<xsl:value-of select="link"/>
					</link>
					<title>
						<xsl:value-of select="title"/>
					</title>
					<description>
						<xsl:value-of select="description"/>
					</description>
					<pubDate>
						<xsl:call-template name="format-date-sort">
							<xsl:with-param name="date" select="pubDate" />
						</xsl:call-template>
					</pubDate>
					</item>
				</xsl:for-each>
			</channel>
		</rss>
	</xsl:template>

	<xsl:template name="format-date-sort">
		<xsl:param name="date" />

		<xsl:variable name="shortDayList" select="string('Sun01Mon02Tue03Wed04Thu05Fri06Sat07')" />
		<xsl:variable name="longDayList" select="string('00Sunday01Monday02Tueday03Wednesday04Thursday05Friday06Saturday07')" />
		<xsl:variable name="shortMonthList" select="string('Jan01Feb02Mar03Apr04May05Jun06Jul07Aug08Sep09Oct10Nov11Dec12')" />
		<xsl:variable name="longMonthList" select="string('00January01February02March03April04May05June06July07August08September09October10November11De cember12')" />

		<xsl:variable name="dayOfWeekName" select="substring-before($date, ',')" />
		<xsl:variable name="day" select="substring-before(substring-after($date, ' '), ' ')" />
		<xsl:variable name="monthName" select="substring-before(substring-after(substring-after($date, ' '), ' '), ' ')" />
		<xsl:variable name="year" select="substring-before(substring-after(substring-after(substring-after($date, ' '), ' '), ' '), ' ')" />
		<xsl:variable name="time" select="substring-before(substring-after(substring-after(substring-after(substring-after($date, ' '), ' '), ' '), ' '), ' ')" />
		<xsl:variable name="zone" select="substring-after(substring-after(substring-after(substring-after(substring-after($date, ' '), ' '), ' '), ' '), ' ')" />

		<xsl:variable name="dayOfWeek" select="substring(substring-after($shortDayList, $dayOfWeekName), 1, 2)" />
		<xsl:variable name="month" select="substring(substring-after($shortMonthList, $monthName), 1, 2)" />
		<xsl:variable name="day2" select="$day" />

		<xsl:variable name="longDayName" select="substring-after(substring-before($longDayList, $dayOfWeek), string(format-number(number($dayOfWeek) - 1, '00')))" />
		<xsl:variable name="longMonthName" select="substring-after(substring-before($longMonthList, $month), string(format-number(number($month) - 1, '00')))" />

		<xsl:variable name="hour" select="substring-before($time, ':')" />
		<xsl:variable name="min" select="substring-before(substring-after($time, ':'), ':')" />

		<xsl:value-of select="concat($year,'-', $month,'-', $day, '-', $hour,'-', $min)" />
	</xsl:template>
</xsl:stylesheet>
