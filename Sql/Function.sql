CREATE FUNCTION dbo.Alphaorder (@str VARCHAR(50))
returns VARCHAR(50)
  BEGIN
      DECLARE @len    INT,
              @cnt    INT =1,
              @str1   VARCHAR(50)='',
              @output VARCHAR(50)=''

      SELECT @len = Len(@str)
      WHILE @cnt <= @len
        BEGIN
            SELECT @str1 += Substring(@str, @cnt, 1) + ','

            SET @cnt+=1
        END

      SELECT @str1 = LEFT(@str1, Len(@str1) - 1)

      SELECT @output += Sp_data
      FROM  (SELECT Split.a.value('.', 'VARCHAR(100)') Sp_data
             FROM   (SELECT Cast ('<M>' + Replace(@str1, ',', '</M><M>') + '</M>' AS XML) AS Data) AS A
                    CROSS APPLY Data.nodes ('/M') AS Split(a)) A
      ORDER  BY Sp_data

      RETURN @output
  END